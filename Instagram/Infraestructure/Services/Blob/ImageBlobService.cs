using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Azure;
using Azure.Storage.Blobs;
using Instagram.config.constants;
using Instagram.config.helpers;
using Instagram.Domain.Repositories.Interfaces.Blob;
using System;

namespace Instagram.Infraestructure.Services.Cloud
{
    public class ImageBlobService : IImageBlobService
    {
        private readonly BlobServiceClient _client;
        private ILogger<ImageBlobService> _logger;
        const string containerName = "images"; // Nombre del contenedor de Blob
        public ImageBlobService(ILogger<ImageBlobService> logger)
        {
            if (EnvironmentConfig.BlobKey is null) throw new NullReferenceException("It's missing blob's key");

            _client = new BlobServiceClient(EnvironmentConfig.BlobKey);
            _logger = logger;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {

            try
            {
                // Parsea la URL para obtener la ruta local del blob
                string path = MediaHelper.GetLocalPath(imageUrl);

                // Obtiene el cliente del contenedor de blobs
                BlobContainerClient containerClient = _client.GetBlobContainerClient(containerName);

                // Obtiene el cliente del blob específico
                BlobClient blobClient = containerClient.GetBlobClient(path);

                // Elimina el blob (video) si existe
                await blobClient.DeleteAsync();

                // Registra un mensaje de información si la eliminación es exitosa
                _logger.LogInformation($"The file {path} has been deleted successfully.");

                return true; // La eliminación fue exitosa
            }
            catch (RequestFailedException ex)
            {
                // Captura y registra cualquier error que ocurra al eliminar el blob
                _logger.LogError($"Error deleting the file: {ex.Message}");

                return false; // La eliminación falló
            }
        }

        public async Task<string> UpdateProfileImageAsync(IFile file, string imageUrl)
        {

            BlobContainerClient containerClient = _client.GetBlobContainerClient(containerName);

            // Obtiene la ruta local del recurso de imagen a partir de la URL proporcionada.
            string path = MediaHelper.GetLocalPath(imageUrl);

            // Crea un cliente de Blob para el recurso de imagen en el contenedor.
            var blobClient = containerClient.GetBlobClient(path);

            using (var stream = file.OpenReadStream())
            {
                // Actualiza el recurso de imagen con el nuevo contenido del archivo proporcionado.
                await blobClient.UploadAsync(stream, true);
            }

            // Devuelve la URI del recurso de imagen actualizado.
            return blobClient.Uri.ToString();
        }


        public async Task<string?> UploadPostImageAsync(IFile file, Guid userId)
        {

            var containerClient = _client.GetBlobContainerClient(containerName);

            // Genera un nombre de blob único para la nueva imagen de post.
            string blobName = $"posts/{userId}/{Guid.NewGuid()}.jpg";

            var blobClient = containerClient.GetBlobClient(blobName);

            const int maxRetryAttempts = 3; // Número máximo de intentos
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var response = await blobClient.UploadAsync(stream, true);
                    }

                    // La imagen se cargó exitosamente, registra un mensaje de información.
                    _logger.LogInformation($"Image was uploaded successfully with URI {blobClient.Uri}");

                    return blobClient.Uri.ToString();
                }
                catch (RequestFailedException ex)
                {
                    // Controla la excepción en caso de un error en la carga del blob.
                    _logger.LogError($"Error uploading image (attempt {currentRetry + 1}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes lanzar la excepción nuevamente o manejarla según tu caso de uso.
                        _logger.LogError("Failed to upload image after multiple attempts.");
                    }
                }
            }

            // En caso de que todos los intentos fallen, puedes lanzar una excepción o manejarla según tus necesidades.
            _logger.LogError("Failed to upload image after multiple attempts.");

            return null;
        }


        public async Task<string> UploadProfileImageAsync(IFile file, Guid userId)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            // Crea un nombre de blob único para la nueva imagen de perfil.
            string blobName = $"profiles/{userId}/{Guid.NewGuid()}.jpg";

            // Crea un cliente de Blob para el nuevo blob en el contenedor.
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                // Carga el contenido del archivo (nueva imagen de perfil) en el blob.
                await blobClient.UploadAsync(stream, true);
            }

            // Devuelve la URI del nuevo blob que contiene la imagen de perfil.
            return blobClient.Uri.ToString();
        }

        public async Task<string?> UploadStoryAsync(IFile file, Guid userId)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            string blobName = $"story/{userId}/{Guid.NewGuid()}.jpg";

            // Crea un cliente de Blob para el nuevo blob en el contenedor.
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                // Carga el contenido del archivo (nueva imagen de perfil) en el blob.
                await blobClient.UploadAsync(stream, true);
            }

            // Devuelve la URI del nuevo blob que contiene la imagen de perfil.
            return blobClient.Uri.ToString();
        }
    }
}

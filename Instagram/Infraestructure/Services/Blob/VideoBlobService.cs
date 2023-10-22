using Azure;
using Azure.Storage.Blobs;
using Instagram.config.constants;
using Instagram.config.helpers;
using Instagram.Domain.Repositories.Interfaces.Blob;

namespace Instagram.Infraestructure.Services.Cloud
{
    public class VideoBlobService : IVideoBlobService
    {
        private readonly BlobServiceClient _client;
        private readonly ILogger<VideoBlobService> _logger;
        const string containerName = "videos";
        public VideoBlobService(ILogger<VideoBlobService> logger) 
        {
            _logger = logger;
            if (EnvironmentConfig.BlobKey is null) throw new NullReferenceException("It's missing blob's key");

            _client = new BlobServiceClient(EnvironmentConfig.BlobKey);
        }

        public async Task<bool> DeleteVideoAsync(string url)
        {
            try
            {
                // Parsea la URL para obtener la ruta local del blob
                string path = MediaHelper.GetLocalPath(url);

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
        public async Task<string?> UploadPostVideoAsync(IFile file, Guid userId)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            string blobName = $"posts/{userId}/{Guid.NewGuid()}.mp4";

            var blobClient = containerClient.GetBlobClient(blobName);

            const int maxRetryAttempts = 3; // Número máximo de intentos
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    // La carga se realizó con éxito en este intento.
                    return blobClient.Uri.ToString();
                }
                catch (RequestFailedException ex)
                {
                    // Controla la excepción en caso de un error en la carga del blob.
                    _logger.LogError($"Error uploading video (attempt {currentRetry + 1}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes lanzar la excepción nuevamente.
                        _logger.LogError("Failed to upload video after multiple attempts.");
                    }
                }
            }

            // En caso de que todos los intentos fallen, lanza una excepción.
            _logger.LogError("Failed to upload video after multiple attempts.");
            return null;
        }

        public async Task<string?> UploadReelAsync(IFile file, Guid userId)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            string blobName = $"reels/{userId}/{Guid.NewGuid()}.mp4";

            var blobClient = containerClient.GetBlobClient(blobName);

            const int maxRetryAttempts = 3; // Número máximo de intentos
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    // La carga se realizó con éxito en este intento.
                    return blobClient.Uri.ToString();
                }
                catch (RequestFailedException ex)
                {
                    // Controla la excepción en caso de un error en la carga del blob.
                    _logger.LogError($"Error uploading reel (attempt {currentRetry + 1}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes lanzar la excepción nuevamente.
                        _logger.LogError("Failed to upload reel after multiple attempts.");
                    }
                }
            }

            // En caso de que todos los intentos fallen, lanza una excepción.
            _logger.LogError("Failed to upload reel after multiple attempts.");
            return null;
        }

        public async Task<string?> UploadStoryAsync(IFile file, Guid userId)
        {

            var containerClient = _client.GetBlobContainerClient(containerName);

            string blobName = $"story/{userId}/{Guid.NewGuid()}.mp4";

            var blobClient = containerClient.GetBlobClient(blobName);

            const int maxRetryAttempts = 3; // Número máximo de intentos
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    // La carga se realizó con éxito en este intento.
                    return blobClient.Uri.ToString();
                }
                catch (RequestFailedException ex)
                {
                    // Controla la excepción en caso de un error en la carga del blob.
                    _logger.LogError($"Error uploading video (attempt {currentRetry + 1}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes lanzar la excepción nuevamente.
                        _logger.LogError("Failed to upload video after multiple attempts.");
                    }
                }
            }

            // En caso de que todos los intentos fallen, lanza una excepción.
            _logger.LogError("Failed to upload video after multiple attempts.");
            return null;
        }
    }
}

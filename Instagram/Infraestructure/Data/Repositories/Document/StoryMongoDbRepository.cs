using Instagram.Domain.Entities.Media;
using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Instagram.Infraestructure.Data.Models.Mongo.Post;
using Instagram.Infraestructure.Data.Models.Mongo.Story;
using Instagram.Infraestructure.Mappers.Story;
using Instagram.Infraestructure.Persistence.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Instagram.Infraestructure.Data.Repositories.Document
{
    public class StoryMongoDbRepository : IStoryMongoDbRepository
    {
        private readonly MongoContext _context;
        private readonly ILogger<StoryMongoDbRepository> _logger;
        public StoryMongoDbRepository(
            MongoContext context,
            ILogger<StoryMongoDbRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string?> CreateStoryAsync(StoryEntity storyEntity)
        {

            int maxRetryAttempts = 3;
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    var collection = _context.Stories;
                    var reelToDb = StoryMapper.MapStoryEntityToStoryDocument(storyEntity);

                    // Intenta insertar el "reel" en la base de datos.
                    await collection.InsertOneAsync(reelToDb);

                    // Obtiene el ID del "reel" insertado.
                    ObjectId reelIdMongo = reelToDb.Id;
                    return reelIdMongo.ToString();
                }
                catch (Exception ex)
                {
                    // Registra un error en caso de fallo y muestra los intentos restantes.
                    _logger.LogError($"Error creating story (attempts remaining: {maxRetryAttempts - currentRetry}): {ex.Message}");

                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes decidir lanzar la excepción nuevamente.
                        _logger.LogError("Failed to create story after multiple attempts.");
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteStoryAsync(string storyId)
        {
            var collection = _context.Stories;

            // Define el filtro para buscar el reel por su ID.
            var filter = Builders<StoryDocument>.Filter.Eq(r => r.Id, new ObjectId(storyId));

            // Realiza la eliminación y obtén el resultado.
            var deleteResult = await collection.DeleteOneAsync(filter);

            if (deleteResult.DeletedCount > 0)
            {
                // El "reel" se eliminó correctamente.
                _logger.LogInformation("Story has been deleted successfully");
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para eliminar.
                _logger.LogWarning("No story found");
                return false;
            }
        }

        public async Task<IEnumerable<StoryEntity>?> GetAllStoriesByIdAsync(string userId)
        {

            // Agrega un registro de eventos (logger) para registrar la entrada al método.
            _logger.LogInformation("Fetching stories for user with ID: {UserId}", userId);

            var collection = _context.Stories;

            var filter = Builders<StoryDocument>.Filter.Eq(story => story.UserId, userId);

            var storyDocuments = await collection.Find(filter).ToListAsync();

            // Define una ordenación para obtener las publicaciones más recientes primero.
            var sort = Builders<StoryDocument>.Sort.Descending(post => post.PublishDate);

            var stories = await collection.Find(filter).Sort(sort).ToListAsync();

            // Ahora mapea los documentos de Story a StoryEntity
            var storyEntities = stories.Select(document =>
            StoryMapper.MapStoryDocumentToStoryEntity(document));

            // Agrega un registro de eventos (logger) para registrar la salida del método.
            _logger.LogInformation("Fetched {Count} stories for user with ID: {UserId}",
                storyEntities.Count(), userId);

            return storyEntities;
        }

        public async Task<StoryEntity?> FindStoryById(string storyId)
        {
            var collection = _context.Stories;

            // Define el filtro para buscar la "story" por su ID.
            var filter = Builders<StoryDocument>.Filter.Eq(r => r.Id, new ObjectId(storyId));

            // Realiza la consulta en la base de datos.
            var storyDocument = await collection.Find(filter).FirstOrDefaultAsync();

            if (storyDocument != null)
            {
                // Si se encontró el documento, mapea y devuelve la entidad de "story" correspondiente.
                _logger.LogInformation($"Fetch story by id : {storyId}");

                return StoryMapper.MapStoryDocumentToStoryEntity(storyDocument);
            }

            // Si no se encuentra un "story" con el ID proporcionado, devuelve null.
            _logger.LogError($"Story not found");
            return null;
        }

        public async Task RemoveExpiredStories()
        {
            var collection = _context.Stories;
            var filter = Builders<StoryDocument>.Filter.Lt(s => s.PublishDate, DateTime.Now.AddHours(-24));
            await collection.DeleteManyAsync(filter);
        }
    }
}

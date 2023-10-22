using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Infraestructure.Data.Models.Mongo.Reel;
using Instagram.Infraestructure.Mappers.Reel;
using Instagram.Infraestructure.Mappers.Shared.Media;
using Instagram.Infraestructure.Persistence.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Instagram.Infraestructure.Data.Repositories.Mongo
{
    public class ReelMongoDbRepository : IReelMongoDbRepository
    {
        private readonly MongoContext _context;
        private readonly ILogger<ReelMongoDbRepository> _logger;
        public ReelMongoDbRepository(
            MongoContext context,
            ILogger<ReelMongoDbRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<string?> AddCommentAsync(CommentEntity commentEntity, string reelId)
        {
            // Obtiene la colección de "reels" desde el contexto.
            var collection = _context.Reels;

            // Define el filtro para encontrar el "reel" específico por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Mapea la entidad de comentario (comment) a un documento de comentario (comment).
            var comment = MediaMapper.MapCommentEntityToCommentDocument(commentEntity);

            // Crea una operación de actualización para agregar el comentario a la lista de comentarios.
            var update = Builders<ReelDocument>.Update.Push(r => r.Comments, comment);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El comentario se agregó correctamente.
                _logger.LogInformation($"A comment has been added to reel with mediaId: {reelId}");

                // Devuelve el CommentId del comentario que se acaba de agregar.
                return comment.CommentId.ToString();
            }
            else
            {
                // No se encontró ningún "reel" para actualizar.
                _logger.LogWarning($"No reel found with mediaId: {reelId}");
                return null;
            }
        }

        public async Task<bool> AddLikeAsync(string mediaId)
        {
            // Obtiene la colección de "reels" desde el contexto de MongoDB.
            var collection = _context.Reels;

            // Crea el filtro para encontrar el "reel" específico por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(mediaId));

            // Crea una operación de actualización para incrementar el contador de "likes" en 1.
            var update = Builders<ReelDocument>.Update.Inc(r => r.Likes, 1);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El "like" se agregó correctamente.
                _logger.LogInformation("A like has been added to the reel with mediaId: {MediaId}", mediaId);
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para actualizar.
                _logger.LogWarning("No reel found with mediaId: {MediaId}", mediaId);
                return false;
            }
        }


        public async Task<bool> AddReplyAsync(ReplyEntity entity, string mediaId, string commentId)
        {
            // Obtén la colección de "reels" desde el contexto.
            var collection = _context.Reels;

            // Crea el filtro para encontrar el "reel" específico por su ID y el comentario específico por su ID.
            var filter = Builders<ReelDocument>.Filter.And(
                Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(mediaId)),
                Builders<ReelDocument>.Filter.ElemMatch(r => r.Comments, c => c.CommentId == new ObjectId(commentId))
            );

            // Encuentra el "reel" y el comentario específico.
            var post = await collection.Find(filter).FirstOrDefaultAsync();

            if (post != null)
            {
                // Encuentra el comentario por su ID.
                var comment = post.Comments?.FirstOrDefault(c => c.CommentId == new ObjectId(commentId));

                if (comment != null)
                {
                    // Mapea la entidad de respuesta (reply) al formato del documento de comentario (comment).
                    var reply = MediaMapper.MapReplyEntityToReplyDocument(entity);

                    // Agrega la respuesta al comentario en memoria.
                    comment.Replies?.Add(reply);

                    // Actualiza el comentario modificado en la colección.
                    var updateResult = await collection.ReplaceOneAsync(filter, post);

                    if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                    {
                        // La respuesta se agregó con éxito.
                        _logger.LogInformation("The reply was added successfully.");
                        return true;
                    }
                }
            }

            // No se pudo agregar la respuesta.
            _logger.LogInformation("Failed to add the reply.");
            return false;
        }
        public async Task<string?> CreateReelAsync(ReelEntity reelEntity)
        {
            int maxRetryAttempts = 3;
            int currentRetry = 0;

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    var collection = _context.Reels;
                    var reelToDb = ReelMapper.MapReelEntityToReelDocument(reelEntity);

                    // Intenta insertar el "reel" en la base de datos.
                    await collection.InsertOneAsync(reelToDb);

                    // Obtiene el ID del "reel" insertado.
                    ObjectId reelIdMongo = reelToDb.Id;
                    return reelIdMongo.ToString();
                }
                catch (Exception ex)
                {
                    // Registra un error en caso de fallo y muestra los intentos restantes.
                    _logger.LogError($"Error creating reel (attempts remaining: {maxRetryAttempts - currentRetry}): {ex.Message}");

                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes decidir lanzar la excepción nuevamente.
                        _logger.LogError("Failed to create reel after multiple attempts.");
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteCommentAsync(string userId, string mediaId, string commentId)
        {
            // Obtiene la colección de "reels" desde el contexto de MongoDB.
            var collection = _context.Reels;

            // Crea el filtro para encontrar el "reel" específico por su ID y el comentario específico por el ID del usuario y el ID del comentario.
            var filter = Builders<ReelDocument>.Filter.And(
                Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(mediaId)),
                Builders<ReelDocument>.Filter.ElemMatch(r => r.Comments,
                c => c.UserId == userId && c.CommentId == new ObjectId(commentId))
            );

            // Crea una operación de actualización para eliminar el comentario específico.
            var update = Builders<ReelDocument>.Update.PullFilter(r => r.Comments,
                c => c.UserId == userId.ToString() && c.CommentId == new ObjectId(commentId));

            // Realiza la actualización y obtén el resultado.
            var result = await collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount > 0)
            {
                // El comentario se eliminó correctamente.
                _logger.LogInformation($"Comment with commentId: {commentId} has been deleted from reel with mediaId: {mediaId}");
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para actualizar o el comentario no se encontró.
                _logger.LogWarning($"No reel found with mediaId: {mediaId} or comment not found with commentId: {commentId}");
                return false;
            }
        }

        public async Task<bool> DeleteLikeAsync(string mediaId)
        {
            // Obtiene la colección de "reels" desde el contexto de MongoDB.
            var collection = _context.Reels;

            // Crea el filtro para encontrar el "reel" específico por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(mediaId));

            // Crea una operación de actualización para incrementar el contador de "likes" en 1.
            var update = Builders<ReelDocument>.Update.Inc(r => r.Likes, -1);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El "like" se agregó correctamente.
                _logger.LogInformation("A like has been remove to the reel with mediaId: {MediaId}", mediaId);
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para actualizar.
                _logger.LogWarning("No reel found with mediaId: {MediaId}", mediaId);
                return false;
            }
        }

        public async Task<bool> DeleteReelAsync(string reelId)
        {
            var collection = _context.Reels;

            // Define el filtro para buscar el reel por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Realiza la eliminación y obtén el resultado.
            var deleteResult = await collection.DeleteOneAsync(filter);

            if (deleteResult.DeletedCount > 0)
            {
                // El "reel" se eliminó correctamente.
                _logger.LogInformation("Reel has been deleted successfully");
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para eliminar.
                _logger.LogWarning("No reel found");
                return false;
            }
        }

        public async Task<bool> DeleteReplyAsync(string reelId, string commentId, ReplyEntity replyEntity)
        {
            // Obtiene la colección de "reels" desde el contexto de MongoDB.
            var collection = _context.Reels;

            // Crea el filtro para encontrar el "reel" específico por su ID y el comentario específico por su ID.
            var filter = Builders<ReelDocument>.Filter.And(
                Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId)),
                Builders<ReelDocument>.Filter.ElemMatch(r => r.Comments,
                c => c.CommentId == new ObjectId(commentId))
            );

            // Encuentra el "reel" y el comentario específico.
            var post = await collection.Find(filter).FirstOrDefaultAsync();

            if (post != null)
            {
                // Encuentra el comentario por su ID.
                var comment = post.Comments?.FirstOrDefault(c => c.CommentId == new ObjectId(commentId));

                if (comment != null)
                {
                    // Encuentra la respuesta específica por su ID y otros criterios relevantes.
                    var replyToRemove = comment.Replies?.FirstOrDefault(r =>
                        r.ReplyId == new ObjectId(replyEntity.ReplyId) &&
                        r.UserId == replyEntity.UserId.ToString()
                    );

                    if (replyToRemove != null)
                    {
                        // Elimina la respuesta de la lista en memoria.
                        comment.Replies?.Remove(replyToRemove);

                        // Actualiza el comentario modificado en la colección.
                        var updateResult = await collection.ReplaceOneAsync(filter, post);

                        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                        {
                            // La respuesta se eliminó con éxito.
                            _logger.LogInformation("The reply was deleted successfully.");
                            return true;
                        }
                    }
                }
            }

            // No se pudo eliminar la respuesta.
            _logger.LogError("Failed to delete the reply.");
            return false;
        }

        public async Task<bool> EditCaptionAsync(string caption, string reelId)
        {
            var collection = _context.Reels;

            // Define el filtro para buscar el reel por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Crea un documento de actualización para establecer el nuevo subtítulo.
            var update = Builders<ReelDocument>.Update.Set(r => r.Description, caption);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El subtítulo se editó correctamente.
                _logger.LogInformation("Caption edited for reel");
                return true;
            }
            else
            {
                // No se encontró ningún reel para actualizar.
                _logger.LogWarning("No reel found");
                return false;
            }
        }
        public async Task<bool> EditLocationAsync(LocationEntity location, string reelId)
        {
            var collection = _context.Reels;

            // Define el filtro para buscar el reel por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Crea un documento de actualización para establecer la nueva ubicación.
            var update = Builders<ReelDocument>.Update.Set(r => r.Location,
                MediaMapper.MapLocationEntityToMapLocationDocument(location));

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // La ubicación se editó correctamente.
                _logger.LogInformation("Location edited for reel");
                return true;
            }
            else
            {
                // No se encontró ningún reel para actualizar.
                _logger.LogWarning("No reel found");
                return false;
            }
        }
        public async Task<bool> EditTagsAsync(List<string> tags, string reelId)
        {
            var collection = _context.Reels;

            // Define el filtro para buscar el "reel" por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Crea un documento de actualización para establecer las nuevas etiquetas.
            var update = Builders<ReelDocument>.Update.Set(r => r.Hashtags, tags);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // Las etiquetas se editaron correctamente.
                _logger.LogInformation("Tags edited for reel");
                return true;
            }
            else
            {
                // No se encontró ningún "reel" para actualizar.
                _logger.LogWarning("No reel found");
                return false;
            }
        }

        public async Task<ReelEntity?> FindReelById(string reelId)
        {
            var collection = _context.Reels;

            // Define el filtro para buscar el "reel" por su ID.
            var filter = Builders<ReelDocument>.Filter.Eq(r => r.Id, new ObjectId(reelId));

            // Realiza la consulta en la base de datos.
            var reelDocument = await collection.Find(filter).FirstOrDefaultAsync();

            if (reelDocument != null)
            {
                // Si se encontró el documento, mapea y devuelve la entidad de "reel" correspondiente.
                _logger.LogInformation($"Fetch reel by id : {reelId}");

               return ReelMapper.MapReelDocumentToReelEntity(reelDocument);
            }

            // Si no se encuentra un "reel" con el ID proporcionado, devuelve null.
            _logger.LogError($"Reel not found");
            return null;
        }


        public async Task<IEnumerable<ReelEntity>?> GetAllReelsByIdAsync(string userId)
        {

            // Agrega un registro de eventos (logger) para registrar la entrada al método.
            _logger.LogInformation("Fetching reels for user with ID: {UserId}", userId);

            var collection = _context.Reels;

            var filter = Builders<ReelDocument>.Filter.Eq(reel => reel.UserId, userId);

            var reelDocuments = await collection.Find(filter).ToListAsync();

            // Ahora mapea los documentos de Post a PostEntity
            var reelEntities = reelDocuments.Select(document =>
            ReelMapper.MapReelDocumentToReelEntity(document));

            // Agrega un registro de eventos (logger) para registrar la salida del método.
            _logger.LogInformation("Fetched {Count} reels for user with ID: {UserId}",
                reelEntities.Count(), userId);

            return reelEntities;
        }

        public async Task<IEnumerable<ReelEntity>?> GetLastReelsIn3Days(string userId)
        {
            // Obtén la colección de documentos de reels.
            var collection = _context.Reels;

            // Calcula la fecha límite de hace 3 días desde la fecha actual.
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            // Define un filtro para buscar reels del usuario en los últimos 3 días.
            var filter = Builders<ReelDocument>.Filter.And(
                Builders<ReelDocument>.Filter.Eq(reel => reel.UserId, userId),
                Builders<ReelDocument>.Filter.Gte(reel => reel.PublishDate, threeDaysAgo)
            );

            // Define una ordenación para obtener los reels más recientes primero.
            var sort = Builders<ReelDocument>.Sort.Descending(reel => reel.PublishDate);

            // Realiza la consulta en la base de datos, aplicando la ordenación.
            var reels = await collection.Find(filter).Sort(sort).ToListAsync();

            if (reels != null && reels.Any())
            {
                // Mapea los documentos de reels a entidades de reels y devuelve la colección.
                var reelEntities = reels.Select(reel => ReelMapper.MapReelDocumentToReelEntity(reel));
                return reelEntities;
            }

            return null;
        }

    }
}

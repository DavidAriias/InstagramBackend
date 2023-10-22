using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Infraestructure.Data.Models.Mongo.Post;
using Instagram.Infraestructure.Mappers;
using Instagram.Infraestructure.Mappers.Shared.Media;
using Instagram.Infraestructure.Persistence.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Instagram.Infraestructure.Data.Repositories.Mongo
{
    public class PostMongoDbRepository : IPostMongoDbRepository
    {
        private readonly MongoContext _context;
        private readonly ILogger<PostMongoDbRepository> _logger;
        public PostMongoDbRepository(
            MongoContext mongoContext,
            ILogger<PostMongoDbRepository> logger)
        {
            _context = mongoContext;
            _logger = logger;
        }

        public async Task<string?> CreatePostAsync(PostEntity post)
        {
            int maxRetryAttempts = 3;
            int currentRetry = 0;
 
            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    var collection = _context.Posts;
                    var postToDb = PostMapper.MapPostDomainToPostDb(post);

                    await collection.InsertOneAsync(postToDb);

                    ObjectId postIdMongo = postToDb.Id;
                    return postIdMongo.ToString();
                }
                catch (Exception ex)
                {
                    // Registra la excepción y el intento actual.
                    _logger.LogError($"Error creating post (attempts remaining: {maxRetryAttempts - currentRetry}): {ex.Message}");

                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        // Espera un tiempo antes de volver a intentarlo.
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        // Si se agotan los intentos, puedes decidir lanzar la excepción nuevamente o notificar sobre el fallo.
                        _logger.LogError("Failed to create post after multiple attempts.");
                    }
                }
            }

            return null;
        }
        public async Task<bool> DeleteCommentAsync(string userId, string mediaId, string commentId)
        {
            
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID y el comentario específico por el ID del usuario y el ID del comentario.
            var filter = Builders<PostDocument>.Filter.And(
                Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(mediaId)),
                Builders<PostDocument>.Filter.ElemMatch(p => p.Comments,
                c => c.UserId == userId && c.CommentId == new ObjectId(commentId))
            );

            // Crea una operación de actualización para eliminar el comentario específico.
            var update = Builders<PostDocument>.Update.PullFilter(p => p.Comments,
                c => c.UserId == userId.ToString() && c.CommentId == new ObjectId(commentId));

            // Realiza la actualización y obtén el resultado.
            var result = await collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount > 0)
            {
                // El comentario se eliminó correctamente.
                _logger.LogInformation($"Comment with commentId: {commentId} has been deleted from post with mediaId: {mediaId}");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar o el comentario no se encontró.
                _logger.LogWarning($"No post found with mediaId: {mediaId} or comment not found with commentId: {commentId}");
                return false;
            }
        }
        public async Task<bool> DeleteLikeAsync(string mediaId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(mediaId));

            // Crea una operación de actualización para reducir el contador de "likes" en 1.
            var update = Builders<PostDocument>.Update.Inc(p => p.Likes, -1);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El "like" se eliminó correctamente.
                _logger.LogInformation($"A like has been deleted from post with mediaId: {mediaId}");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar.
                _logger.LogWarning($"No post found with mediaId: {mediaId}");
                return false;
            }
        }
        public async Task<string?> AddCommentAsync(CommentEntity commentEntity, string postId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId));

            var comment = MediaMapper.MapCommentEntityToCommentDocument(commentEntity);

            // Crea una operación de actualización para agregar el comentario a la lista de comentarios.
            var update = Builders<PostDocument>.Update.Push(p => p.Comments, comment);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El comentario se agregó correctamente.
                _logger.LogInformation($"A comment has been added to post with mediaId: {postId}");

                // Devuelve el CommentId del comentario que se acaba de agregar.
                return comment.CommentId.ToString();
            }
            else
            {
                // No se encontró ninguna publicación para actualizar.
                _logger.LogWarning($"No post found with mediaId: {postId}");
                return null;
            }
        }
        public async Task<bool> AddLikeAsync(string mediaId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(mediaId));

            // Crea una operación de actualización para incrementar el contador de "likes" en 1.
            var update = Builders<PostDocument>.Update.Inc(p => p.Likes, 1);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // El "like" se agregó correctamente.
                _logger.LogInformation("A like has been added to the post with mediaId: {MediaId}", mediaId);
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar.
                _logger.LogWarning("No post found with mediaId: {MediaId}", mediaId);
                return false;
            }
        }
        public async Task<bool> EditCaptionAsync(string caption, string postId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId));

            // Crea una operación de actualización para establecer la nueva descripción.
            var update = Builders<PostDocument>.Update.Set(p => p.Description, caption);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // La descripción se editó correctamente.
                _logger.LogInformation("Caption edited for post");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar.
                _logger.LogWarning("No post found");
                return false;
            }
        }
        public async Task<bool> EditLocationAsync(LocationEntity location, string postId)
        {
            var collection = _context.Posts;

            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId));

            var update = Builders<PostDocument>.Update.Set(p => p.Location,
                MediaMapper.MapLocationEntityToMapLocationDocument(location));

            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // La ubicación se editó correctamente
                _logger.LogInformation("Location edited for post");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar
                _logger.LogWarning("No post found");
                return false;
            }
        }
        public async Task<bool> EditTagsAsync(List<string> tags, string postId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId));

            // Crea una operación de actualización para establecer las nuevas etiquetas.
            var update = Builders<PostDocument>.Update.Set(p => p.Hashtags, tags);

            // Realiza la actualización y obtén el resultado.
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount > 0)
            {
                // Las etiquetas se editaron correctamente.
                _logger.LogInformation("Tags edited for post");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para actualizar.
                _logger.LogWarning("No post found");
                return false;
            }
        }
        public async Task<bool> DeletePostAsync(string postId)
        {
            var collection = _context.Posts;

            // Crea un filtro para encontrar la publicación específica por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId));

            // Realiza la eliminación y obtén el resultado.
            var deleteResult = await collection.DeleteOneAsync(filter);

            if (deleteResult.DeletedCount > 0)
            {
                // La publicación se eliminó correctamente.
                _logger.LogInformation($"Post has been deleted successfully");
                return true;
            }
            else
            {
                // No se encontró ninguna publicación para eliminar.
                _logger.LogWarning($"No post found");
                return false;
            }
        }
        public async Task<bool> AddReplyAsync(ReplyEntity entity, string postId, string commentId)
        {
            // Obtener la colección de publicaciones (posts) desde el contexto

            var collection = _context.Posts;

            // Crear el filtro para encontrar la publicación específica por su ID y el comentario específico por su ID
            var filter = Builders<PostDocument>.Filter.And(
                Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId)),
                Builders<PostDocument>.Filter.ElemMatch(p => p.Comments, c => c.CommentId == new ObjectId(commentId))
            );

            // Obtener la publicación y el comentario específico
            var post = await collection.Find(filter).FirstOrDefaultAsync();

            if (post != null)
            {

                var comment = post.Comments?.FirstOrDefault(c => c.CommentId == new ObjectId(commentId));
                if (comment != null)
                {
                    // Mapear la entidad de respuesta (reply) al formato de documento de comentario (comment)
                    var reply = MediaMapper.MapReplyEntityToReplyDocument(entity);

                    // Agregar la respuesta al comentario en memoria
                    comment.Replies?.Add(reply);

                    // Actualizar el comentario modificado en la colección
                    var updateResult = await collection.ReplaceOneAsync(filter, post);

                    if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                    {
                        _logger.LogInformation("The reply was added successfully.");
                        return true;
                    }
                }
            }

            _logger.LogInformation("Failed to add the reply.");
            return false;
        }
        public async Task<bool> DeleteReplyAsync(string postId, string commentId, ReplyEntity replyEntity)
        {
            // Obtener la colección de publicaciones (posts) desde el contexto de MongoDB
            var collection = _context.Posts;

            // Crear el filtro para encontrar la publicación específica por su ID y el comentario específico por su ID
            var filter = Builders<PostDocument>.Filter.And(
                Builders<PostDocument>.Filter.Eq(p => p.Id, new ObjectId(postId)),
                Builders<PostDocument>.Filter.ElemMatch(p => p.Comments, c => c.CommentId == new ObjectId(commentId))
            );

            // Obtener la publicación y el comentario específico
            var post = await collection.Find(filter).FirstOrDefaultAsync();

            if (post != null)
            {
                var comment = post.Comments?.FirstOrDefault(c => c.CommentId == new ObjectId(commentId));
                if (comment != null)
                {
                    // Encontrar el reply específico por su ID y otros criterios relevantes
                    var replyToRemove = comment.Replies?.FirstOrDefault(r =>
                        r.ReplyId == new ObjectId(replyEntity.ReplyId) &&
                        r.UserId == replyEntity.UserId.ToString()
                    );

                    if (replyToRemove != null)
                    {
                        // Eliminar el reply de la lista en memoria
                        comment.Replies?.Remove(replyToRemove);

                        // Actualizar el comentario modificado en la colección
                        var updateResult = await collection.ReplaceOneAsync(filter, post);

                        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                        {
                            _logger.LogInformation("The reply was deleted successfully.");
                            return true;
                        }
                    }
                }
            }

            _logger.LogError("Failed to delete the reply.");
            return false;
        }
        public async Task<IEnumerable<PostEntity>?> GetAllPostsByIdAsync(string userId)
        {
            // Agrega un registro de eventos (logger) para registrar la entrada al método.
            _logger.LogInformation("Fetching posts for user with ID: {UserId}", userId);

            var collection = _context.Posts;

            var filter = Builders<PostDocument>.Filter.Eq(post => post.UserId, userId);

            var postDocuments = await collection.Find(filter).ToListAsync();

            // Ahora mapea los documentos de Post a PostEntity, dependiendo de cómo lo tengas configurado
            var postEntities = postDocuments.Select(document => PostMapper.MapPostDocumentToPostEntity(document));

            // Agrega un registro de eventos (logger) para registrar la salida del método.
            _logger.LogInformation("Fetched {Count} posts for user with ID: {UserId}", postEntities.Count(), userId);

            return postEntities;
        }

        public async Task<PostEntity?> FindPostByIdAsync(string postId)
        {
            // Agrega un registro de eventos (logger) para registrar la entrada al método.
            _logger.LogInformation("Fetching post by ID: {PostId}", postId);

            // Obtén la colección de documentos de publicaciones.
            var collection = _context.Posts;

            // Crea un filtro para buscar una publicación por su ID.
            var filter = Builders<PostDocument>.Filter.Eq(post => post.Id, new ObjectId(postId));

            // Ejecuta la consulta y obtén el primer documento de publicación que coincida con el filtro.
            var postDocument = await collection.Find(filter).FirstOrDefaultAsync();

            // Verifica si se encontró una publicación y, si es así, mapea el documento de Post a PostEntity.
            if (postDocument != null)
            {
                var postEntity = PostMapper.MapPostDocumentToPostEntity(postDocument);

                // Agrega un registro de eventos (logger) para registrar la salida del método.
                _logger.LogInformation("Fetched post by ID: {PostId}", postId);

                return postEntity;
            }
            else
            {
                // Si no se encontró una publicación, agrega un registro de eventos (logger) y devuelve null.
                _logger.LogInformation("Post not found with ID: {PostId}", postId);
                return null;
            }
        }

        public async Task<IEnumerable<PostEntity>?> GetLastPostsIn3DaysAsync(string userId)
        {
            // Obtén la colección de documentos de publicaciones.
            var collection = _context.Posts;

            // Calcula la fecha límite de hace 3 días desde la fecha actual.
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            // Define un filtro para buscar publicaciones del usuario en los últimos 3 días.
            var filter = Builders<PostDocument>.Filter.And(
                Builders<PostDocument>.Filter.Eq(post => post.UserId, userId),
                Builders<PostDocument>.Filter.Gte(post => post.PublishDate, threeDaysAgo)
            );

            // Define una ordenación para obtener las publicaciones más recientes primero.
            var sort = Builders<PostDocument>.Sort.Descending(post => post.PublishDate);

            // Realiza la consulta en la base de datos, aplicando la ordenación.
            var posts = await collection.Find(filter).Sort(sort).ToListAsync();

            if (posts != null && posts.Any())
            {
                // Mapea los documentos de publicaciones a entidades de publicaciones y devuelve la colección.
                var postEntities = posts.Select(post => PostMapper.MapPostDocumentToPostEntity(post));
                return postEntities;
            }

            return null;
        }

    }
}

using Instagram.Domain.Entities.User;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Data.Models.SQL;
using Instagram.Infraestructure.Mappers.User;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Instagram.Infraestructure.Data.Repositories.SQL
{
    public class UserSQLDbRepository : IUserSQLDbRepository
    {
        private readonly InstagramContext _context;
        public UserSQLDbRepository(InstagramContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> CreateUser(UserEntity user)
        {
            // Mapeamos la entidad de usuario a una entidad de datos de usuario.
            var userDataDB = UserMapper.MapUserEntityToUserDatum(user);

            // Agregamos la entidad de datos de usuario a la base de datos.
            var userDb = await _context.UserData.AddAsync(userDataDB);

            // Guardamos los cambios en la base de datos y obtenemos el resultado.
            var result = await _context.SaveChangesAsync();

            // Si se guardó con éxito (result == 1), actualizamos el ID del usuario y lo devolvemos.
            if (result == 1)
            {
                user.Id = userDb.Entity.Id;
                return user;
            }

            // Si no se guardó con éxito, devolvemos null.
            return null;
        }

        public async Task<UserEntity?> CreateUserDescription(UserEntity user)
        {
            // Mapear la entidad de usuario a una entidad de descripción de usuario.
            var userToDb = UserMapper.MapUserEntityToUserDescription(user);

            // Mapear la fecha de nacimiento del usuario a una entidad de fecha de nacimiento.
            var birthday = UserMapper.MapUserEntityToBirthday(user);

            // Mapear el nombre de usuario a una entidad de nombre de usuario.
            var userUsernameDb = UserMapper.MapUserEntityToUserName(user);

            // Verificar si el nombre de usuario no es nulo y agregarlo a la base de datos.
            if (user.Name is not null)
            {
                await _context.UserNames.AddAsync(userUsernameDb);
            }

            // Agregar la entidad de descripción de usuario a la base de datos.
            await _context.UserDescriptions.AddAsync(userToDb);

            // Agregar la entidad de fecha de nacimiento a la base de datos.
            await _context.Birthdays.AddAsync(birthday);

            // Guardar los cambios en la base de datos y obtener el resultado.
            var result = await _context.SaveChangesAsync();

            // Si se guardó con éxito (result == 1), devolver la entidad de usuario; de lo contrario, devolver null.
            return result == 1 ? user : null;
        }

        public async Task<UserEntity?> FindUserByEmail(string email)
        {
            // Buscamos un usuario en la base de datos cuyo correo electrónico coincida con el proporcionado.
            var user = await _context.UserData.FirstOrDefaultAsync(u => u.Email == email);

            // Si se encuentra un usuario, mapeamos la entidad de datos de usuario a una entidad de usuario y la devolvemos.
            if (user is not null)
            {
                return UserMapper.MapUserDatumToUserEntity(user);
            }

            // Si no se encuentra ningún usuario con el correo electrónico proporcionado, devolvemos null.
            return null;
        }
        public async Task<UserEntity?> FindUserById(Guid id)
        {
            // Buscamos un usuario en la base de datos cuyo ID coincida con el proporcionado.
            var user = await _context.UserData.FirstOrDefaultAsync(u => u.Id == id);

            // Si se encuentra un usuario, mapeamos la entidad de datos de usuario a una entidad de usuario y la devolvemos.
            if (user is not null)
            {
                return UserMapper.MapUserDatumToUserEntity(user);
            }

            // Si no se encuentra ningún usuario con el ID proporcionado, devolvemos null.
            return null;
        }

        public async Task<UserEntity?> FindUserByUsername(string username)
        {
            // Buscamos un usuario en la base de datos cuyo nombre de usuario coincida con el proporcionado.
            var user = await _context.UserData.FirstOrDefaultAsync(u => u.Username == username);

            // Si se encuentra un usuario, mapeamos la entidad de datos de usuario a una entidad de usuario y la devolvemos.
            if (user is not null)
            {
                return UserMapper.MapUserDatumToUserEntity(user);
            }

            // Si no se encuentra ningún usuario con el nombre de usuario proporcionado, devolvemos null.
            return null;
        }

        public async Task<DateTime> GetLastBirthdayChangeDate(Guid userId)
        {
            // Consulta la base de datos para obtener la fecha de la última modificación de la fecha de nacimiento del usuario.
            return await _context.Birthdays
                .Where(u => u.UserId == userId)
                .Select(u => u.Updatedate)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateBirthday(DateOnly birthday, Guid userId)
        {
            var usuario = await _context.Birthdays.FirstOrDefaultAsync(u => u.UserId == userId);

            if (usuario is not null)
            {
                // Actualizar los campos de Birthdaydate y Updatedate
                usuario.Birthdaydate = birthday;
                usuario.Updatedate = DateTime.Now;

                // Guardar los cambios en la base de datos y comprobar si se actualizó con éxito.
                int result = await _context.SaveChangesAsync();

                // Devolver true si la actualización tuvo éxito, de lo contrario, false.
                return result > 0;
            }

            // Si no se encontró el usuario, devolver false.
            return false;
        }

        public async Task<bool> UpdateLink(LinkEntity link, Guid userId)
        {
            var usuario = await _context.UserLinks.FirstOrDefaultAsync(u => u.UserId == userId);

            if (usuario is not null)
            {
                // Actualiza un enlace existente si se encuentra.
                UpdateExistingLink(usuario, link);
            }
            else
            {
                // Crea un nuevo enlace si no se encuentra el usuario.
                await CreateNewLink(link, userId);
            }

            // Guarda los cambios en la base de datos y devuelve true si la actualización tuvo éxito.
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }

        private static void UpdateExistingLink(UserLink usuario, LinkEntity link)
        {
            usuario.Title = link.Title;
            usuario.Link = link.Link;
        }

        private async Task CreateNewLink(LinkEntity link, Guid userId)
        {
            var userToDb = new UserLink
            {
                Link = link.Link,
                Title = link.Title,
                UserId = userId
            };

            await _context.UserLinks.AddAsync(userToDb);
        }

        public async Task<bool> UpdateBiography(string bio, Guid userId)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user is not null)
            {
                user.Description = bio;
                int result = await _context.SaveChangesAsync();
                return result > 0; // Devuelve true si la actualización tuvo éxito.
            }

            return false; // Si no se encontró el usuario, devuelve false.
        }

        public async Task<bool> UpdateName(string name, Guid userId)
        {
            var userToDb = new UserName
            {
                Name = name,
                UserId = userId
            };

            // Agregamos el nombre de usuario a la base de datos.
            await _context.UserNames.AddAsync(userToDb);

            // Guardamos los cambios en la base de datos y comprobamos si se realizó con éxito.
            int result = await _context.SaveChangesAsync();

            // Devolvemos true si la actualización tuvo éxito, de lo contrario, false.
            return result > 0;
        }


        public async Task<bool> UpdatePassword(string pass, Guid userId)
        {
            var user = await _context.UserData.FirstOrDefaultAsync(user => user.Id == userId);

            if (user is not null)
            {
                user.Password = pass;
                int result = await _context.SaveChangesAsync();
                return result > 0; // Devuelve true si la actualización tuvo éxito.
            }

            return false; // Si no se encontró el usuario, devuelve false.
        }

        public async Task<bool> UpdateUsername(string username, Guid userId)
        {
            var user = await _context.UserData.FirstOrDefaultAsync(user => user.Id == userId);

            if (user is not null)
            {
                user.Username = username;
                int result = await _context.SaveChangesAsync();
                return result > 0; // Devuelve true si la actualización tuvo éxito.
            }

            return false; // Si no se encontró el usuario, devuelve false.
        }
        public int GetNumberChangesOnName(Guid userId)
        {
            DateTime time = DateTime.Now.AddDays(-14.0);

            return _context.UserNames
           .Where(c => c.UserId == userId && c.Updatedate >= time)
           .Count();
        }

        public async Task<bool> UpdatePronoun(string pronoun, Guid userId)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(user => user.UserId == userId);

            if (user is not null)
            {
                user.Pronoun = pronoun;
                int result = await _context.SaveChangesAsync();
                return result > 0; // Devuelve true si la actualización tuvo éxito.
            }

            return false; // Si no se encontró el usuario, devuelve false.
        }

        public async Task<UserEntity?> FindUserByPhoneNumber(string phoneNumber)
        {
            var user = await _context.UserData.FirstOrDefaultAsync(user => user.Phonenumber == phoneNumber);

            // Si se encuentra un usuario, mapeamos la entidad de datos de usuario a una entidad de usuario y la devolvemos.
            if (user is not null)
            {
                return UserMapper.MapUserDatumToUserEntity(user);
            }

            // Si no se encuentra ningún usuario con el nombre de usuario proporcionado, devolvemos null.
            return null;
        }

        public async Task<UserEntity?> GetAllDataAboutUserById(Guid userId)
        {
            var userStoredProcedure =  await _context.UserStoredProcedures
            .FromSqlRaw("SELECT * FROM get_user_data_by_id(@user_in_id)",
            new NpgsqlParameter("user_in_id", userId))
            .FirstOrDefaultAsync();

           if (userStoredProcedure is not null)
            {
                return UserMapper.MapUserStoredProcedureToUserEntity(userStoredProcedure);
            }

            return null;
            
        }

        public async Task<bool> UpdateIsPrivateProfile(Guid userId)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(user => user.UserId == userId);

            if (user is not null)
            {
                user.Isprivated = !user.Isprivated;
                await _context.SaveChangesAsync();
                return true; // La actualización se realizó con éxito.
            }

            return false; // No se encontró el usuario, por lo que la actualización no se realizó.
        }
        public async Task<bool> UpdateIsVerificateProfile(Guid userId)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(user => user.UserId == userId);

            if (user is not null)
            {
                user.Isverificated = !user.Isverificated;
                await _context.SaveChangesAsync();
                return true; // La actualización se realizó con éxito.
            }

            return false; // No se encontró el usuario, por lo que la actualización no se realizó.
        }

        public async Task<bool> UpdateImageProfile(Guid userId, string url)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(user => user.UserId == userId);

            if (user is not null)
            {
                user.Imageprofile = url;
                await _context.SaveChangesAsync();
                return true; // La actualización se realizó con éxito.
            }

            return false; // No se encontró el usuario, por lo que la actualización no se realizó.
        }

        public async Task<string?> FindImageProfileById(Guid userId)
        {
            var user = await _context.UserDescriptions.FirstOrDefaultAsync(user => user.UserId == userId);

            if (user is not null) {
                return user.Imageprofile;
            }
            return null;
        }
    }
}

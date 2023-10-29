# Instagram
En el siguiente repositorio, se encuentra desarrollado el backend para app como la de instagram utilizando arquitectura limpia
<h2>Backend</h2>
<div>
<img src="https://th.bing.com/th/id/R.2b5bab6427a23b349185833ef09f3eb7?rik=YUgKKzVwy8kGkw&riu=http%3a%2f%2fasr-danesh.ir%2fuploads%2flogo-c-asp-net_0.jpg&ehk=njP9r7ffvZtb6Zw%2bMdxg8X2%2f6GdXakOlIXsHz0x7SKw%3d&risl=&pid=ImgRaw&r=0" width="300", height="150">
 <img src="https://th.bing.com/th/id/R.6710f0d23dc0341c696ada0274d5bc25?rik=N75aHNWr2X0yRw&pid=ImgRaw&r=0" width="250", height="150">
</div>

Desarrollado en C# con ASP .NET Core para hacer una API de tipo GraphQL, utilizando librerias como:
<li>Hot chocolate para utilizar GraphQL en C#</li>
<li>EntityFramework para el mapeo de base de datos relacional</li>
<li>MongoDB driver para mapear los objetos de MongoDB</li>
<li>Neo4j driver para conectarse con Neo4j</li>
<li>Dot Env para el manejo de variables de entorno</li>
<li>Bcrypt.Net para encriptar las contraseñas de los users</li>
<li>Linq para realizar consultas</li>
<li>JwtBearer para generar Json Web Tokens tanto de access token como refresh token</li>
<li>Twilio para el manejo de SMS de verificación</li>
<li>Spotify API para poner musica a los reels, posts, stories, etc</li>

<h2>Base de datos</h2>
<div>
  <img src="https://www.muylinux.com/wp-content/uploads/2017/10/postgresql.png" width="250", height="150">
  <img src="https://mechomotive.com/wp-content/uploads/2021/06/MongoDB_Logo-1024x536.jpg" width="250", height="150">
  <img src="https://yt3.ggpht.com/a/AATXAJwTBrMWg8ErJHShesf-6CXMU_o0mE1IlNerGQ=s900-c-k-c0xffffffff-no-rj-mo" width="150", height="150">
  <img src="https://images.g2crowd.com/uploads/product/image/social_landscape/social_landscape_1489695934/redis.jpg" width="250", height="150">  
</div>
<li>PostgresSQL para guardar datos estructurados como los datos relacionados con el usuario ,access token y refresh token, datos personales de usuario,etc</li>
<li>MongoDB para guardar los posts, reels, comentarios, etc de los usuarios</li>
<li>Neo4j para guardar las relaciones de los usuarios como los seguidores,seguidos, recomendaciones de seguidores,recomendaciones de posts,etc</li>
<li>Redis para hacer el caché de los datos más solicitados por los usuarios como posts, reels, etc. Así no se satura las db's al realizar multiples peticiones y mejorar el UX</li>

<h2>Servicios de Cloud</h2>
<h3>La mayor parte del ecosistema cloud esta en Azure, la app esta alojada en Azure</h3>
<img src="https://th.bing.com/th/id/R.4e172cfde6a77d2f66cf52276ed2f565?rik=Elh%2fBcZYEdV67A&riu=http%3a%2f%2fwindowsgeek.lk%2fwp-content%2fuploads%2f2019%2f06%2fazure.jpg&ehk=wdVuOzreveU%2b0gFi7VOeEvvjWaNh9PdcI01%2bwd6NDcQ%3d&risl=&pid=ImgRaw&r=0" width="250", height="150">
<li>Azure blob para almacenar las imagenes de perfil, posts, reels e stories de los usuarios</li>
<li>Azure hub notifications para las push notifications a los dispositivos de los usuarios</li>

# Pokedex Scraper

Este proyecto consta de dos componentes: un scraper escrito en C# para obtener información detallada de todos los Pokémon existentes y almacenarlos en MongoDB Atlas, y un proyecto en Node.js para realizar las peticiones a la base de datos.

## Requisitos previos

Asegúrate de tener instalado lo siguiente en tu máquina:

- .NET Core SDK
- Node.js
- MongoDB Atlas

## Scraper en C#

El scraper está escrito en C# y se encarga de obtener los datos de los Pokémon desde una fuente externa y almacenarlos en MongoDB Atlas.

## Configuración del scraper

1. Clona este repositorio en tu máquina:
    ```shell
   git clone https://github.com/RETBOT/Pokedex_Api.git

2. Abre el proyecto del scraper en tu IDE de preferencia (por ejemplo Visual Studio).

3. Configura la conexión a MongoDB Atlas:
    En el archivo Scraper/Pokedex_Api.cs, modifica la URL de conexión a MongoDB Atlas con tus propias credenciales y detalles de la base de datos.

## Uso del scraper
1. Ejecuta el scraper para obtener los datos de los Pokémon y almacenarlos en MongoDB Atlas.
2. Asegúrate de que MongoDB Atlas esté en funcionamiento.
3. En tu IDE, ejecuta el proyecto del scraper. Esto iniciará el proceso de scraping y almacenará los datos en la base de datos en MongoDB Atlas.

# Proyecto Node.js
El proyecto Node.js se encarga de realizar las peticiones a MongoDB Atlas y proporcionar una interfaz para acceder a los datos de los Pokémon.

## Configuración del proyecto Node.js
1. Clona este repositorio en tu máquina:
    ```shell
    git clone https://github.com/RETBOT/Pokedex_Api_Nodejs.git

2. En la carpeta del proyecto Node.js, instala las dependencias ejecutando el siguiente comando en la terminal:
    ```shell
    npm install

3. Configura la conexión a MongoDB Atlas:
Asegúrate de tener una cuenta en MongoDB Atlas y haber creado un clúster.
En el archivo config.js, modifica la URL de conexión a MongoDB Atlas con tus propias credenciales y detalles de la base de datos.

## Uso del proyecto Node.js
Asegúrate de que MongoDB Atlas esté en funcionamiento.
En la carpeta del proyecto Node.js, ejecuta el siguiente comando en la terminal:
    ```shell
    npm start

Esto iniciará el servidor Node.js y podrás realizar las peticiones a la base de datos.

# Contribución
Si deseas contribuir a este proyecto, puedes hacerlo siguiendo estos pasos:

Haz un fork de este repositorio.
Crea una rama (git checkout -b feature/nueva-funcionalidad).
Realiza los cambios necesarios y realiza commit de tus cambios (git commit -am 'Agrega nueva funcionalidad').
Envía tus cambios a tu repositorio remoto (git push origin feature/nueva-funcionalidad).
Abre un pull request en este repositorio.

# Licencia
Este proyecto se encuentra bajo la Licencia MIT. Para más información
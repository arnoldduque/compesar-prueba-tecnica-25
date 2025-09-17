# Prueba Técnica - Compesar - Sep. 2025
## Dev Challenge – API Génesis
### Consigna
El desafío consiste en crear un servicio web que exponga un endpoint para consultar capítulos y versos del Libro de Génesis de la Biblia.
### Reglas de negocio
- El servicio debe permitir obtener un capítulo completo con todos sus versos.
- Si se solicita el capítulo N, el servicio debe retornar todos los capítulos desde el 1 hasta el N (por ejemplo, pedir el capítulo 2 devuelve los capítulos 1 y 2).
- La API debe mantener un buen desempeño incluso si se solicitan varios capítulos.
### Fuente de datos
Se debe consumir la API pública bible-api.com
- Todos los capítulos de Génesis: (https://bible-api.com/data/web/GEN)
- Capítulo específico (ejemplo, capítulo 1): (https://bible-api.com/data/web/GEN/1)
### Ejemplo de respuesta esperada del servicio
```json
{
	" Genesis ": [
		{
			"chapter": 1,
			"verses": [
				{
					"verse": 1,
					"text": "In the beginning, God created the heavens and the earth.\n"
				},
				{
					"verse": 31,
					"text": "God saw everything that he had made \n"
				}
			]
		},
		{
			"chapter": 2,
			"verses": [
				{
					"verse": 1,
					"text": "The heavens, the earth, and all their vast array were finished.\n"
				},
				{
					"verse": 2,
					"text": "On the seventh day God finished his work which….\n"
				}
			]
		}
	]
}
```
### Requisitos técnicos
- Microservicio
- Debe exponer al menos un endpoint:
  - GET /genesis/{capitulo}
- /genesis/1 → Retorna capítulo 1 con todos sus versos.
- /genesis/2 → Retorna capítulos 1 y 2 con todos sus versos.
- /genesis/N → Retorna capítulos 1 … N con todos sus versos.
### Base de datos
- Los datos obtenidos desde la API pública deben ser persistidos en la base de datos en cada consulta (no se permite precargar datos).
- La base de datos puede ser relacional o no relacional (a elección).
- Debe ejecutarse en un contenedor Docker (puedes usar una imagen existente, no es necesario crear Dockerfile).
- Debe ser posible acceder a la tabla/colección cargada con los datos.
### Documentación
- La solución debe entregarse en un repositorio Git público (GitHub o GitLab).
- La última fecha de modificación del repositorio debe ser la fecha máxima de entrega del challenge.
- El repositorio debe incluir al menos un README.md con instrucciones claras:
  - Cómo construir y ejecutar el servicio.
  - Cómo levantar el contenedor de base de datos.
  - Cómo consumir los endpoints expuestos.
### Testing (opcional pero valorado)
- No es obligatorio.
- Se valorará positivamente la inclusión de tests unitarios.
### ¿Qué se va a evaluar?
- Uso adecuado de módulos y librerías (evitar dependencias innecesarias).
- Buenas prácticas de código y patrones de diseño (Clean Code, inyección de dependencias, separación de responsabilidades).
- Estructura clara y modularidad del proyecto (“que los cambios no duelan”).
- Performance del API frente a múltiples capítulos.
- Claridad y completitud para poder ejecutar la solución de principio a fin (build del servicio, base de datos y endpoints).
- Repositorio público accesible y actualizado en la fecha límite.

## Documentación
### Como contruir y ejecutar el servicio
- Descargar e instalar la ultima versión de [.NET 8](https://dotnet.microsoft.com/en-us/download). Verificar la version con el siguiente comando: ``` dotnet --version ```
- Descargar e instalar la ultima versión de [Docker](https://docs.docker.com/desktop/setup/install/windows-install/). Seleccionar la configuracion con maquinas Linux. Verificar la version con el siguiente commando: ``` docker --version ```
- Descagar el repositorio.
- En una consola con permisos de adminitrador, ir a la raiz del proyecto donde esta ubicado el archivo **bibliaAPI.sln** y ejecutar los siguientes comandos:
  - Para verificar que el proyecto se descargo correctamente: ``` dotnet build ```
  - Para construir la imagen de Docker: ```docker build -t bibliaapi:1.0 .```
  - Para montar el contenedor de la imagen: ```docker run -p 5000:8080 bibliaapi:1.0```
### Como consumir los servicios expuestos
Si los anteriores comandos se ejecutan correctamente, el servicio ya esta disponible y puede ser consumido con diferentes ejemplos:
- (http://localhost:5000/Genesis/1)
- (http://localhost:5000/Genesis/2)
- (http://localhost:5000/Genesis/20)
### Como acceder a la base de datos
La base de datos esta montada sobre SQLite3, de modo que el modelo relacional se puede acceder a traves de un archivo en la raiz del contenedor llamado **biblia.db** de la siguiente forma:
- Mientras el contenedor aun esta ejecutandose con los pasos anteriores y luego de haber llamado varios de los endpoints, se debe abrir una nueva ventana de comando con permisos de administrador.
- Listamos todos los contendores con el comando: ```docker ps``` 
  
| CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES |
| --- | --- | --- | --- | --- | --- | --- |
| 7711d0f71163|bibliaapi:1.0 | "dotnet bibliaAPI.dll" | 46 seconds ago | Up 46 seconds | 0.0.0.0:5000->8080/tcp, [::]:5000->8080/tcp | xenodochial_gould |

- Tomamos el identificador del contenedor para llamar al Bash: ``` docker exec -it 7711d0f71163 bash ```
- Una vez estemos adentro del Bash, ingresamos el comando para abrir el archivo de base de datos: ``` sqlite3 biblia.db ```
- En el promtp de SQLite3 ya podemos verificar la informacion de las tablas: ``` select * from Libro; ``` 

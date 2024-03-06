# E-Commerce 

E-Commerce application developed for the .NET laboratory in University, alongside: @StirbuLarisa, @tudor-tess, @radeanuroxana17

The project is made using the .NET framework on the backend, adopting a serverless approach, through Azure Fuctions. On the frontend, we used the Blazor framework, as a requirement from the lead professor.
It features most of the needed functionalities for such a platform to function, meaning: Account management in a secure way, selling and buying items, clasification of items and so on.

**Important:**
- Docker is needed in order to run the database, the backend and the integration tests. Without these 3 in docker containers, the application will not work.
- You might need to manually kill the SQL service if it runs before you start the database from Docker, as it may claim the port first.
- Sendgrid, the API used for mail sending, blocks the API key once it detects it has gone public. Due to the public nature of this repo, an API key is required to be changed once the project is pulled onto the local machine.

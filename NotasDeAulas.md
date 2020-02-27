#                   ► Notas Balta 1976


.NET - Plataforma de desenvolvimento que rodo sobre o Common Language Runtime (CLR)


• Arquitetura

    Frameworks Desenvolvimento (ASP.NET, Web Services, ADO.NET)
        > Base Class Libraru (BCL)
            > CLR - 
                Código é compilado para CLR em tempo de execução tranformado em bits

• Compiladores

    .NET Compiler Platform (Roslyn) - OpenSource
        API que Compila C# e VB, pode ser utilizado em qualquer SO. 

    .NET Native
        Compila C# para o código nativo do Windows C++.

Self Host - A aplicação é o próprio servidor Web.


## Iniciando Projeto

dotnet new web -n ProductCatalogApi -f netcoreapp2.2


## Começando

Models - Representam o banco de dados em objetos


## EF Core (ORM - Mapeamento Objeto Relacional)

    Faz a relação do banco de dados relacional com os objetos.

    • Instalação
        
        dotnet add package Microsoft.EntityFrameworkCore


### DataContext (Contexto de Dados - Representação do banco de dados em memória)

    - Única exigência do EF Core.
    - É onde se cria todas as entidades e os mapeamentos.



    - Criar arquivo 'StoreDataContext.cs' na pasta Data

    • Pode-se Usar a conexão do context no StoreDataContext mesmo usando o metodo de configuração do EF: 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\\MSSQLLocalDB; Database=Store; User ID=;Password=;");
        }

        Na Startup:

        // Resolvendo dependencia do Context para ser injetado nas controllers e services
        services.AddScoped<StoreDataContext, StoreDataContext>();

    • Ou então usando o appSettings.Json e a registrando o Context na Startup:

        - StoreDataContext:

            public StoreDataContext(DbContextOptions<StoreDataContext> options)
           : base(options)
            {
            }

        - appSettings.Json:

            {
                "ConnectionStrings": {
                "StoreDataContext": "Server=(localdb)\\MSSQLLocalDB; Database=Store; User ID=;Password=;"
                }
            }

        - Startup

            Adicionar construtor com a injeção do Iconfiguration:

                public Startup(IConfiguration configuration)
                {
                    Configuration = configuration;
                }

                public IConfiguration Configuration { get; }

            
            Adicionar no ConfigureServices:
            
                services.AddDbContext<StoreDataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("StoreDataContext")));


### Usando CodeFirst no EF Core

    • Adicionar no csproj extensão do CLI para poder usar 'dotnet ef ...'

        <ItemGroup>
            <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.Dotnet" Version="2.0.0" />
        </ItemGroup>

        Ou

        - dotnet add package Microsoft.EntityFrameworkCore.Tools.Dotnet


    • Adicionando a primeira Migration

        - dotnet ef migrations add initial

    • Refletindo no banco

        - dotnet ef database update



    Da maneira que está o EF cria as tabelas no plural e não restringe os tamanhos dos campos, para mudar isso: 

        - Adicionar o mapeamento com as configurações diretamente no Context:

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Product>()
                .ToTable("Product");

                modelBuilder.Entity<Category>()
                    .ToTable("Category");
            }

        Ou então,
        
        - Criar past Maps na pasta Data e adicionar as configurações de cada modelo.

        - Adicionar no StoreDataContext.cs para que toda vez que o EF Core criar o banco ele usar as regras do mapeamento:

            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.ApplyConfiguration(new ProductMap());
                builder.ApplyConfiguration(new CategoryMap());
            }


### Criação da API


####   ► Middlewares

    São agentes que são chamados em toda Request/Response.

#### Arquitetura MVC (Model-View-Controller)

    É um padrão para organização do projeto.
    É utilizado por várias outras linguagens.
    Simples e Objetivo.

    • Instalando (Ou nem precisa)

        -  dotnet add package Microsoft.AspNetCore.Mvc

    Na Startup:

        -  services.AddMvc();

        - app.UseMvc();

#### Rotas

    Podemos receber parâmetros pela URL (Query String) ou pelo Corpo da Requisição (Body)

    • Verbos HTTP

        GET (Apenas Cabeçalho) - Obter
        POST - Inserir
        PUT - Atualizar
        DELETE - Excluir


    • Roteamento (Sempre no plural - Boa prática)

        GET - /v1/products
            Lista os Produtos

        GET  - /v1/products/255
            Lista o Produto 255

        GET  - /v1/products/255/images
            Lista as imagens do Produto 255

        POST - /v1/products
            Cria um novo produto
        
        PUT - /v1/products
            Atualiza um produto

        DELETE - /v1/products
            Exclui um produto


#### ViewModels

    Adequa as informações para a tela ou retorno da api.

    Ex: Quando na tela tem o campo senha e o campo confirmar senha.

    ► Criando ViewModels apenas com os dados necessários:

        - Criar ListProductViewModel que irá ser utilizada para exibir os produtos.

        - Criar EditorProductViewModel que irá ser utilizada quando um produto for inserido ou alterado. 


#### Fail Fast Validations (Falhar o mais rápido possivel) usando Flunt:

    - Valida as ViewModels de Entrada
    - Armazena todos os erros
    - Retorna a falha antes de prosseguir

    ► Instalando:

        - dotnet add package Flunt


    ► Implementando:

        Herdar modelo Ex:

            public class EditorProductViewModel : Notifiable, IValidatable

        Adicionar Metodo Validate no modelo:

            public void Validate()
            {
                AddNotifications(
                    new Contract()
                        .HasMaxLen(Title, 120, "Title", "O título deve conter até 120 caracteres")
                        .HasMinLen(Title, 3, "Title", "O título deve conter pelo menos 3 caracteres")
                        .IsGreaterThan(Price, 0, "Price", "O preço deve ser maior que zero")
                );
            }


#### Repository Pattern (Design Pattern)

    Cada entidade/modelo tem seu repositório.
    Abstrai o acesso ao banco de dados (Remove da Controller).


    - Criar pasta Repositories
    - Criar classe ProductRepository.cs
    - Adicionar classes que acessam os dados e remove-los da controller
    - Mudar a injeção da controller para o ProductRepository
    
    - Adicionando dependencia do ProductRepository para a ProductController na Startup:

            services.AddTransient<ProductRepository, ProductRepository>();

                -> Usa-se AddTransient pois toda vez que se pedir um novo ProductRepository ele deve criar uma nova instância do ProductRepository.


#### Queries no EF Core

    - Usamos LINQ (Language Integrated Query) para fazer Queries no banco.
    - Devemos retornar uma ViewModel ao invés de uma Model para trazer só o que precisamos.
    - Utilizar Sempre AsNoTracking().


#### Versionamento

    - Usá-se versionamente para se poder fazer uma alteração no end point ou modelos sem afetar os apps que consomem essa api.


##### Cache

    - Pode-se usar cache no front ou no back (server).
    - Pode-se usar quando certos dados não mudam ou mudam com pouca frequência.

    ► Usando:

        - Adicionar anotação em cima do endpoint

        [ResponseCache(Duration = 60)] // Irá verificar no Server

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 60)] // Irá verificar direto no client


#### Compressão da Resposta

    ► Instalando:

        - dotnet add package Microsoft.AspNetCore.ResponseCompression

    ► Implementando:

        Na Startup, metodo ConfigureServices:

            // Middleware de Compressao
            // Compressiona as respostas
            services.AddResponseCompression();

        Na Startup, metodo Configure:

            app.UseResponseCompression();


#### Documentação 

      ► Instalando:

        - dotnet add package Microsoft.

    ► Implementando:

        Na Startup, metodo ConfigureServices:

           

        Na Startup, metodo Configure:

          

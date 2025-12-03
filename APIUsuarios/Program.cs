using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(cs);
});

// DI
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migrations automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// ENDPOINTS -----------------------------------------

app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    return Results.Ok(await service.ListarAsync(ct));
});

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
});

app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IValidator<UsuarioCreateDto> validator,
    IUsuarioService service,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
        return Results.BadRequest(validation.Errors);

    try
    {
        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Email"))
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IValidator<UsuarioUpdateDto> validator,
    IUsuarioService service,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
        return Results.BadRequest(validation.Errors);

    try
    {
        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var ok = await service.RemoverAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.Run();

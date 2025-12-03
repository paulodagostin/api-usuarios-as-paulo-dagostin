using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);

        return usuarios.Select(u => new UsuarioReadDto(
            u.Id,
            u.Nome,
            u.Email,
            u.DataNascimento,
            u.Telefone,
            u.Ativo,
            u.DataCriacao
        ));
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        var email = dto.Email.ToLower();

        // idade mínima
        if (CalcularIdade(dto.DataNascimento) < 18)
            throw new InvalidOperationException("Usuário deve ter pelo menos 18 anos.");

        // email único
        if (await _repository.EmailExistsAsync(email, ct))
            throw new InvalidOperationException("Email já cadastrado.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = email,
            Senha = dto.Senha,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario is null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        var email = dto.Email.ToLower();

        if (CalcularIdade(dto.DataNascimento) < 18)
            throw new InvalidOperationException("Usuário deve ter pelo menos 18 anos.");

        var outro = await _repository.GetByEmailAsync(email, ct);
        if (outro is not null && outro.Id != id)
            throw new InvalidOperationException("Email já cadastrado.");

        usuario.Nome = dto.Nome;
        usuario.Email = email;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario is null) return false;

        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        return await _repository.EmailExistsAsync(email.ToLower(), ct);
    }

    private static int CalcularIdade(DateTime data)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - data.Year;
        if (data.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }
}

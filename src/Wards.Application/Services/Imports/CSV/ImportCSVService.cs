﻿using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text;
using Wards.Application.Services.Imports.Shared.Models.Input;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.Services.Imports.CSV;

/// <summary>
/// A classe tem o intuito de ser mais dinâmica possível;
/// Mas, para que ela funcione corretamente, todos as propriedades do parâmetro FileCsvInput devem ser preenchidos corretamente;
/// (!!!) E o mais importante: os campos do .csv (IFormFile) devem estar na ordem EXATA da classe alvo (object TargetClass);
/// </summary>
/// 
/// <example>
/// Exemplo de uso:
/// <code>
/// input.TargetClass = new NomeDaClasse();
/// input.TableNameToBulkInsert = "NomeDaTabela";
/// var list = _importCsv.ReadCsv(input);
/// </code>
/// </example>
public sealed class ImportCSVService : IImportCSVService
{
    private readonly WardsContext _context;

    public ImportCSVService(WardsContext context)
    {
        _context = context;
    }

    public async Task ImportarCSV(ImportCSVInput input)
    {
        if (input.FormFile is null || input.FormFile.Length == 0)
        {
            throw new Exception("O arquivo a ser importado é inválido e/ou está corrompido");
        }

        Type tipoPropClasseAlvo = input.ClasseAlvo!.GetType();
        string csv = LerCSV(input.FormFile);
        List<object?> listaObjetoFinal = IterarConteudo(csv, input, tipoPropClasseAlvo);

        if (!listaObjetoFinal.Any())
        {
            throw new Exception("Houve um problema interno. Aparentemente nenhum registro foi encontrado no CSV para ser salvo na base de dados");
        }

        await _context.AddRangeAsync(listaObjetoFinal!);
        await _context.SaveChangesAsync();
    }

    private static string LerCSV(IFormFile formFile)
    {
        Encoding encoding;
        using (var stream = formFile.OpenReadStream())
        {
            encoding = DetectarEncoding(stream);
            stream.Close();
        }

        if (encoding != Encoding.UTF8 && encoding != Encoding.Latin1)
            throw new Exception("Encoding inválido");

        using MemoryStream memoryStream = new();
        formFile.CopyTo(memoryStream);
        memoryStream.Position = 0;

        using StreamReader reader = new(memoryStream, encoding);
        string csvContent = reader.ReadToEnd();

        return csvContent;
    }

    private static List<object?> IterarConteudo(string csv, ImportCSVInput input, Type tipoPropClasseAlvo)
    {
        string[] linhas = csv.Split('\n');

        PropertyInfo[] propriedades = tipoPropClasseAlvo.GetProperties();
        List<object?> listaObjetoFinal = new();

        int i = 0;
        foreach (var linha in linhas)
        {
            if (input.IsPularCabecalho && i == 0)
            {
                i++;
                continue;
            }

            string[] dados = linha.Split(input.Separador);
            object novaInstanciaObjetoAlvo = Activator.CreateInstance(tipoPropClasseAlvo)!;

            int j = 0;
            foreach (var prop in propriedades)
            {
                if (j >= dados.Length)
                {
                    break;
                }

                bool isKeyProperty = IsKey(prop);
                bool isForeignKey = IsForeignKey(prop);
                bool isNotMapped = IsNotMapped(prop);

                if (!prop.CanWrite || isKeyProperty || isForeignKey || isNotMapped)
                {
                    continue;
                }

                object? dadoNormalizado = NormalizarDado(dados[j], prop.PropertyType);
                prop.SetValue(novaInstanciaObjetoAlvo, dadoNormalizado);
                j++;
            }

            listaObjetoFinal.Add(novaInstanciaObjetoAlvo);
            i++;
        }

        return listaObjetoFinal;
    }

    private static object? NormalizarDado(object dado, Type tipo)
    {
        if (dado is null)
        {
            // Retornar um valor padrão para tipos por valor (int, double, etc.);
            if (tipo.IsValueType)
            {
                return Activator.CreateInstance(tipo)!;
            }

            // Se o tipo for uma classe, apenas retorna null;
            return null;
        }

        if (tipo == typeof(string))
        {
            string? decodedString = CorrigirLetrasOutroEncoding(dado.ToString());
            return Convert.ChangeType(decodedString, tipo);
        }

        // Convertendo o objeto para o tipo especificado;
        return Convert.ChangeType(dado, tipo);
    }

    private static string? CorrigirLetrasOutroEncoding(string? text)
    {
        var replacements = new Dictionary<string, string>
        {
            { "Ã¡", "á" },
            { "Ã©", "é" },
            { "Ã­", "í" },
            { "Ã³", "ó" },
            { "Ãº", "ú" },
            { "Ã", "Á" },
            { "Ã‰", "É" },
            { "Ã", "Í" },
            { "Ã“", "Ó" },
            { "Ãš", "Ú" },
            { "Ã£", "ã" },
            { "Ãµ", "õ" },
            { "Ã±", "ñ" },
            { "Ã", "Ç" },
            { "Ã§", "ç" },
            { "Ã¼", "ü" },
            { "Ã¶", "ö" },
            { "Ã¤", "ä" },
            { "Ã€", "À" },
            { "Ãˆ", "È" },
            { "ÃŒ", "Ì" },
            { "Ã’", "Ò" },
            { "Ã™", "Ù" },
            { "Ã¢", "â" },
            { "Ãª", "ê" },
            { "Ã®", "î" },
            { "Ã´", "ô" },
            { "Ã»", "û" }
        };
    
        foreach (var replacement in replacements)
        {
            text = text?.Replace(replacement.Key, replacement.Value);
        }
    
        return text;
    }
}

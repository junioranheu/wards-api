using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Reflection;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.Services.Exports.PDF
{
    /// <summary>
    /// Helper para geração de PDF com a biblioteca QuestPDF.
    /// A biblioteca QuestPDF NUNCA deve ser atualizada, porque a versão instalada ([2022.12.15]) é gratuita para uso comercial. Atualizar o pacote pode causar problemas com licença.
    /// Mensagem da biblioteca: "If you are an existing QuestPDF user and for any reason cannot update, you can stay with the 2022.12.X release with the extended quality support but without any new features, improvements, or optimizations. That release will always be available under the MIT license, free for commercial usage. We are planning to sunset support for the 2022.12.X branch around Q1 2024. Until then, it will continue to receive quality and bug-fix updates.".
    /// </summary>
    public static class QuestPDFService
    {
        public static float DefinirConfiguracoesIniciais(this PageDescriptor pagina, PageSize tipoPagina, bool isPortrait = true, int margem = 50, string backgroundColor = Colors.White, int fontSize = 16)
        {
            if (isPortrait)
            {
                pagina.Size(tipoPagina.Portrait());
            }
            else
            {
                pagina.Size(tipoPagina.Landscape());
            }

            pagina.Margin(margem);
            pagina.PageColor(backgroundColor);
            pagina.DefaultTextStyle(x => x.FontSize(fontSize));

            float pageWidth = isPortrait ? tipoPagina.Width : tipoPagina.Height;
            float pageWidthFinal = pageWidth - margem;
            return pageWidthFinal;
        }

        public static void TextoTabela(this TableDescriptor tabela, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool hasBorder = true, bool isCenter = true, int fontSize = 12)
        {
            var celula = isCenter ? // Workaround;
                tabela.Cell().Border((hasBorder ? 1 : 0)).BorderColor((hasBorder ? Colors.Grey.Lighten1 : Colors.Transparent)).AlignCenter().AlignMiddle().Text(conteudo).FontSize(fontSize) :
                tabela.Cell().Border((hasBorder ? 1 : 0)).BorderColor((hasBorder ? Colors.Grey.Lighten1 : Colors.Transparent)).Text(conteudo).FontSize(fontSize);

            if (isBold)
            {
                celula.Bold();
            }

            if (!string.IsNullOrEmpty(fontColor))
            {
                celula.FontColor(fontColor);
            }

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                celula.BackgroundColor(backgroundColor);
            }
        }

        public static void TextoAlternativo(this ColumnDescriptor coluna, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0, bool isAlignRight = false)
        {
            TextoAlternativoConstructor(coluna.Item(), conteudo, isBold, fontColor, backgroundColor, isSmall, paddingTop, paddingBottom, paddingTopCustom, paddingBottomCustom, isAlignRight);
        }

        public static void TextoAlternativo(this RowDescriptor item, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0, bool isAlignRight = false)
        {
            TextoAlternativoConstructor(item.RelativeItem(), conteudo, isBold, fontColor, backgroundColor, isSmall, paddingTop, paddingBottom, paddingTopCustom, paddingBottomCustom, isAlignRight);
        }

        private static void TextoAlternativoConstructor(QuestPDF.Infrastructure.IContainer item, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0, bool isAlignRight = false)
        {
            var containerItemNormalizado = item.PaddingTop((paddingTop ? 16 : 0)).PaddingBottom((paddingBottom ? 16 : 0));

            if (paddingTopCustom > 0)
            {
                containerItemNormalizado.PaddingTop(paddingTopCustom);
            }

            if (paddingBottomCustom > 0)
            {
                containerItemNormalizado.PaddingTop(paddingBottomCustom);
            }

            if (isAlignRight)
            {
                containerItemNormalizado.AlignRight();
            }

            var itemNormalizado = containerItemNormalizado.Text(conteudo);

            if (isBold)
            {
                itemNormalizado.Bold();
            }

            if (!string.IsNullOrEmpty(fontColor))
            {
                itemNormalizado.FontColor(fontColor);
            }

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                itemNormalizado.BackgroundColor(backgroundColor);
            }

            if (isSmall)
            {
                itemNormalizado.FontSize(9);
            }
        }

        public static void DefinirColunas(this TableDescriptor tabela, List<string> cabecalhos, float tamanhoColunasFixo = 0, List<float>? listaTamanhosColunas = default, float pageWidth = 0, bool hasBorder = true, bool isCenter = true, int fontSize = 12, int offSetCustomWidthAutomatico = 5)
        {
            // Caso a função tenha um valor no parâmetro "tamanhoColunasFixo", todas as colunas terão o mesmo width;
            if (tamanhoColunasFixo > 0)
            {
                tabela.ColumnsDefinition(coluna =>
                {
                    foreach (var item in cabecalhos)
                    {
                        coluna.ConstantColumn(tamanhoColunasFixo);
                    }
                });
            }
            // Caso a lista do parâmetro "listaTamanhosColunas" NÃO tenha valores, defina automaticamente o width das colunas;
            else if (listaTamanhosColunas?.Count == 0 || listaTamanhosColunas == null)
            {
                float width = (pageWidth / cabecalhos.Count) - offSetCustomWidthAutomatico;

                tabela.ColumnsDefinition(coluna =>
                {
                    foreach (var item in cabecalhos)
                    {
                        coluna.ConstantColumn(width);
                    }
                });
            }
            // Caso a lista do parâmetro "listaTamanhosColunas" TENHA valores (else), defina o width das colunas com base nessa lista;
            else
            {
                tabela.ColumnsDefinition(coluna =>
                {
                    if (listaTamanhosColunas?.Count != cabecalhos.Count)
                    {
                        throw new Exception($"Problema interno. A quantidade de itens das listas não se coincidem. [QuestPDFService/{ObterNomeDoMetodo()}]");
                    }

                    foreach (var item in listaTamanhosColunas!)
                    {
                        coluna.ConstantColumn(item);
                    }
                });
            }

            // Estilização das colunas;
            tabela.Header(coluna =>
            {
                foreach (var item in cabecalhos)
                {
                    // Workaround;
                    if (isCenter)
                    {
                        coluna.Cell().Border((hasBorder ? 1 : 0)).BorderColor((hasBorder ? Colors.Grey.Lighten1 : Colors.Transparent)).AlignCenter().AlignMiddle().Text(item).FontSize(fontSize).Bold();
                    }
                    else
                    {
                        coluna.Cell().Border((hasBorder ? 1 : 0)).BorderColor((hasBorder ? Colors.Grey.Lighten1 : Colors.Transparent)).Text(item).FontSize(fontSize).Bold();
                    }
                }
            });
        }

        public static void TituloSessao(this ColumnDescriptor coluna, string conteudo, bool paddingTop = false, bool paddingBottom = false, bool isLarge = false)
        {
            var celula = coluna.Item().
                         PaddingTop((paddingTop ? 16 : 0)).PaddingBottom((paddingBottom ? 16 : 0)).
                         BorderBottom(1).BorderTop(1).BorderColor(Colors.LightBlue.Medium).AlignCenter().AlignMiddle().
                         Text(conteudo).FontColor(Colors.LightBlue.Medium).FontSize(16).Bold();

            if (isLarge)
            {
                celula.FontSize(22);
            }
        }

        public static void TextoFixadoTopoTodasPags(this PageDescriptor pagina, string conteudo, string fontColor = Colors.Grey.Darken4)
        {
            pagina.Header().AlignLeft().PaddingBottom(16).Text(conteudo).SemiBold().FontSize(16).FontColor(fontColor);
        }

        [Obsolete]
        public static void LogoFixadoTopoTodasPags(this PageDescriptor pagina, string conteudoFallback, string imgUrl = "Assets\\Images\\logo.png", string fontColor = Colors.Grey.Darken4)
        {
            try
            {
                string? outPutDirectory = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
                int binIndex = outPutDirectory!.IndexOf("bin", StringComparison.OrdinalIgnoreCase);

                if (binIndex < 0)
                {
                    throw new Exception();
                }

                string outputFinal = outPutDirectory[..binIndex];
                string img = Path.Combine(outputFinal, imgUrl);
                string relLogo = new Uri(img).LocalPath;

                if (!File.Exists(relLogo))
                {
                    throw new Exception();
                }

                pagina.Header().Container().Width(250).PaddingBottom(16).Stack(stack =>
                {
                    stack.Item().Image(relLogo);
                });
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                pagina.TextoFixadoTopoTodasPags(conteudoFallback, fontColor);
            }
        }

        public static string FormatarDouble(double? valor)
        {
            if (!valor.HasValue)
            {
                return string.Empty;
            }

            return valor.Value.ToString("F2");
        }

        public static string ObterDataExtensa()
        {
            return DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
        }

        public static string FormatarData_ddMyyHHmm(DateTime data)
        {
            return data.ToString("dd/M/yy HH:mm");
        }

        public static void QuebrarPagina(this ColumnDescriptor coluna)
        {
            coluna.Item().PageBreak();
        }

        public static void AdicionarFooterComInfosDasPags(this PageDescriptor pagina)
        {
            pagina.Footer().AlignCenter().Text(text =>
            {
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }

        public static List<List<double?>> DefinirListaDinamicaParaGerarMedias(List<string> cabecalhos)
        {
            List<List<double?>> listaDinamica_ParaGerarMedias = new();

            for (int i = 0; i < cabecalhos.Count; i++)
            {
                listaDinamica_ParaGerarMedias.Add(new List<double?>());
            }

            return listaDinamica_ParaGerarMedias;
        }

        public static void Gerar_E_Exibir_Medias(this TableDescriptor tabela, List<List<double?>> listaDinamica_ParaGerarMedias)
        {
            foreach (var item in listaDinamica_ParaGerarMedias)
            {
                double? average = item.Count > 0 ? item.Average() : 0.0;
                tabela.TextoTabela(conteudo: FormatarDouble(average), isBold: true, fontColor: Colors.LightBlue.Medium);
            }
        }
    }
}
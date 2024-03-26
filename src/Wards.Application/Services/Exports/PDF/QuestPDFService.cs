using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Wards.Application.Services.Exports.PDF
{
    /// <summary>
    /// Helper para geração de PDF com a biblioteca QuestPDF.
    /// A biblioteca QuestPDF NUNCA deve ser atualizada, porque a versão instalada ([2022.12.15]) é gratuita para uso comercial. Atualizar o pacote pode causar problemas com licença.
    /// Mensagem da biblioteca: "If you are an existing QuestPDF user and for any reason cannot update, you can stay with the 2022.12.X release with the extended quality support but without any new features, improvements, or optimizations. That release will always be available under the MIT license, free for commercial usage. We are planning to sunset support for the 2022.12.X branch around Q1 2024. Until then, it will continue to receive quality and bug-fix updates.".
    /// </summary>
    public static class QuestPDFService
    {
        public static void Texto(this TableDescriptor tabela, string? conteudo, int borda = 1, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent)
        {
            var celula = tabela.Cell().Border(borda).BorderColor(Colors.Grey.Darken1).AlignCenter().AlignMiddle().Text(conteudo);

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

        public static void TextoAlternativo(this ColumnDescriptor coluna, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0)
        {
            TextoAlternativoConstructor(coluna.Item(), conteudo, isBold, fontColor, backgroundColor, isSmall, paddingTop, paddingBottom, paddingTopCustom, paddingBottomCustom);
        }

        public static void TextoAlternativo(this RowDescriptor item, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0)
        {
            TextoAlternativoConstructor(item.RelativeItem(), conteudo, isBold, fontColor, backgroundColor, isSmall, paddingTop, paddingBottom, paddingTopCustom, paddingBottomCustom);
        }

        private static void TextoAlternativoConstructor(QuestPDF.Infrastructure.IContainer item, string? conteudo, bool isBold = false, string fontColor = Colors.Black, string backgroundColor = Colors.Transparent, bool isSmall = false, bool paddingTop = false, bool paddingBottom = false, int paddingTopCustom = 0, int paddingBottomCustom = 0)
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

        public static void DefinirColunas(this TableDescriptor tabela, List<string> cabecalhos, int tamanhoColuna = 0, float pageWidth = 0)
        {
            float width = tamanhoColuna;

            if (width == 0)
            {
                int offset = 3;
                width = (pageWidth / cabecalhos.Count) - offset;
            }

            tabela.ColumnsDefinition(coluna =>
            {
                foreach (var item in cabecalhos)
                {
                    coluna.ConstantColumn(width);
                }
            });

            tabela.Header(coluna =>
            {
                foreach (var item in cabecalhos)
                {
                    coluna.Cell().Border(1).AlignCenter().AlignMiddle().Text(item).Bold();
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
            pagina.Header().AlignLeft().Text(conteudo).SemiBold().FontSize(16).FontColor(fontColor);
        }

        public static float DefinirConfiguracoesIniciais(this PageDescriptor pagina, PageSize tipoPagina, bool isPortrait = true, int margem = 50, string backgroundColor = Colors.White, int fontSize = 11)
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
    }
}
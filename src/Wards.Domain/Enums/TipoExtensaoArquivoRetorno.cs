﻿using System.ComponentModel;

namespace Wards.Domain.Enums
{
    public enum TipoExtensaoArquivoRetorno
    {
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        XLSX = 1,

        [Description("text/csv")]
        CSV = 2
    }
}
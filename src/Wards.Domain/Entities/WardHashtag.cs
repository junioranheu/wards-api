﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class WardHashtag
    {
        [Key]
        public int WardHashtagId { get; set; }

        public int? WardId { get; set; }
        [NotMapped]
        public Ward? Wards { get; init; }

        public int? HashtagId { get; set; }
        public Hashtag? Hashtags { get; init; }

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}
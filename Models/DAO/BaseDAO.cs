using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonAPI.Models.DAOs
{
    public class BaseDAO
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
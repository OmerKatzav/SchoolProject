using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server;

internal class Media
{
    [Key, Column("id")]
    public required Guid Id { get; init; }

    [Required, MaxLength(255), Column("name")]
    public required string Name { get; init; }

    [Required, Column("length")]
    public required double Length { get; init; }

    [Required, Column("seconds_per_chunk")]
    public required double SecondsPerChunk { get; init; }

    [Required, Column("chunk_sizes")]
    public required int[,] ChunkSizes { get; init; }

    [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed), Column("search_vector")]
    public NpgsqlTsVector SearchVector { get; init; } = null!;
}
#!/bin/bash

set -e

SERVICE_NAME="MediaService"
ENTITY_NAME="Media"
ENTITY_NAME_LOWER="media"

BASE_DIR="./Services/$SERVICE_NAME"
DOMAIN_DIR="$BASE_DIR/$SERVICE_NAME.Domain/Entities"
CONFIG_DIR="$BASE_DIR/$SERVICE_NAME.Domain/Configurations"
DTO_DIR="./Shared/Contracts/Media"
DBCONTEXT_FILE="$BASE_DIR/$SERVICE_NAME.Infrastructure/Persistence/${SERVICE_NAME}DbContext.cs"

mkdir -p "$DOMAIN_DIR" "$CONFIG_DIR" "$DTO_DIR"

# 1. Entity
cat > "$DOMAIN_DIR/$ENTITY_NAME.cs" <<EOF
namespace $SERVICE_NAME.Domain.Entities;

public class $ENTITY_NAME
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string UploadedBy { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
EOF

# 2. DTO
cat > "$DTO_DIR/${ENTITY_NAME}Dto.cs" <<EOF
namespace Shared.Contracts.Media;

public class ${ENTITY_NAME}Dto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string UploadedBy { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}
EOF

# 3. Configuration
cat > "$CONFIG_DIR/${ENTITY_NAME}Configuration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using $SERVICE_NAME.Domain.Entities;

namespace $SERVICE_NAME.Domain.Configurations;

public class ${ENTITY_NAME}Configuration : IEntityTypeConfiguration<$ENTITY_NAME>
{
    public void Configure(EntityTypeBuilder<$ENTITY_NAME> builder)
    {
        builder.ToTable("${ENTITY_NAME}s");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FileName).IsRequired().HasMaxLength(255);
        builder.Property(m => m.Url).IsRequired();
        builder.Property(m => m.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Size).IsRequired();
        builder.Property(m => m.UploadedBy).IsRequired().HasMaxLength(100);
    }
}
EOF

# 4. Register in DbContext
if ! grep -q "DbSet<$ENTITY_NAME>" "$DBCONTEXT_FILE"; then
  echo "    public DbSet<$ENTITY_NAME> ${ENTITY_NAME}s { get; set; } = null!;" >> "$DBCONTEXT_FILE"
  echo "✅ DbSet<$ENTITY_NAME> added to $DBCONTEXT_FILE"
else
  echo "⚠️  DbSet<$ENTITY_NAME> already exists in $DBCONTEXT_FILE"
fi

echo "✅ $ENTITY_NAME entity, DTO, and configuration generated for $SERVICE_NAME"

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace gardenrush.lib.Data
{
    public partial class GardenRushDbContext : DbContext
    {
        public GardenRushDbContext()
        {
        }

        public GardenRushDbContext(DbContextOptions<GardenRushDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<History> History { get; set; }
        public virtual DbSet<HistoryPiece> HistoryPiece { get; set; }
        public virtual DbSet<Piece> Piece { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("game");

                entity.Property(e => e.GameId).HasColumnType("int(11)");

                entity.Property(e => e.NGameStatus)
                    .HasColumnName("nGameStatus")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("history");

                entity.HasIndex(e => e.GameId)
                    .HasName("GameId");

                entity.Property(e => e.HistoryId).HasColumnType("int(11)");

                entity.Property(e => e.BReloadTruck).HasColumnName("bReloadTruck");

                entity.Property(e => e.GameId)
                    .HasColumnName("GameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NActionType)
                    .HasColumnName("nActionType")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NDestPos)
                    .HasColumnName("nDestPos")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NPlayer)
                    .HasColumnName("nPlayer")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NScore1)
                    .HasColumnName("nScore1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NScore2)
                    .HasColumnName("nScore2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NSourcePos)
                    .HasColumnName("nSourcePos")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("history_ibfk_1");
            });

            modelBuilder.Entity<HistoryPiece>(entity =>
            {
                entity.HasKey(e => new { e.HistoryId, e.PieceId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.PieceId)
                    .HasName("Constr_HistoryPiece_Piece_fk");

                entity.Property(e => e.HistoryId).HasColumnType("int(11)");

                entity.Property(e => e.PieceId).HasColumnType("int(11)");

                entity.HasOne(d => d.History)
                    .WithMany(p => p.HistoryPiece)
                    .HasForeignKey(d => d.HistoryId)
                    .HasConstraintName("Constr_HistoryPiece_History_fk_2");

                entity.HasOne(d => d.Piece)
                    .WithMany(p => p.HistoryPiece)
                    .HasForeignKey(d => d.PieceId)
                    .HasConstraintName("Constr_HistoryPiece_Piece_fk");
            });

            modelBuilder.Entity<Piece>(entity =>
            {
                entity.ToTable("piece");

                entity.HasIndex(e => e.GameId)
                    .HasName("GameId");

                entity.Property(e => e.PieceId).HasColumnType("int(11)");

                entity.Property(e => e.GameId)
                    .HasColumnName("GameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HashIndex).HasColumnType("int(11)");

                entity.Property(e => e.NOwner)
                    .HasColumnName("nOwner")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NPieceStatus)
                    .HasColumnName("nPieceStatus")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NPieceType)
                    .HasColumnName("nPieceType")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NPosition)
                    .HasColumnName("nPosition")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Piece)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("piece_ibfk_1");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("player");

                entity.HasIndex(e => e.GameId)
                    .HasName("GameId");

                entity.Property(e => e.PlayerId).HasColumnType("int(11)");

                entity.Property(e => e.GameId)
                    .HasColumnName("GameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Identity)
                    .IsRequired()
                    .HasColumnType("varchar(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.NPlayer)
                    .HasColumnName("nPlayer")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NPlayerStatus)
                    .HasColumnName("nPlayerStatus")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NScore)
                    .HasColumnName("nScore")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("players_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.GameId1)
                    .HasColumnName("GameID1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GameId2)
                    .HasColumnName("GameID2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GameId3)
                    .HasColumnName("GameID3")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GameId4)
                    .HasColumnName("GameID4")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Identity)
                    .IsRequired()
                    .HasColumnType("varchar(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

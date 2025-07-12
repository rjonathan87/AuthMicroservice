using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Infrastructure.Data;

public partial class AuthDbContext : DbContext
{
    public AuthDbContext()
    {
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<JwtToken> JwtTokens { get; set; }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Server=localhost;Database=AuthenticationDB;User Id=sa;Password=t2i86t7D9X;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JwtToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__JwtToken__658FEEEA8493A911");

            entity.HasIndex(e => e.TokenIdentifier, "IX_JwtTokens_TokenIdentifier");

            entity.HasIndex(e => new { e.UserId, e.ExpirationDate }, "IX_JwtTokens_UserId_ExpirationDate");

            entity.HasIndex(e => e.TokenIdentifier, "UQ__JwtToken__328826BA360C42A1").IsUnique();

            entity.Property(e => e.Reason).HasMaxLength(256);
            entity.Property(e => e.RevokedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TokenIdentifier).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.JwtTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JwtTokens_Users");
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__LoginHis__4D7B4ABD3A148119");

            entity.ToTable("LoginHistory");

            entity.HasIndex(e => e.LoginTimestamp, "IX_LoginHistory_LoginTimestamp").IsDescending();

            entity.HasIndex(e => new { e.UserId, e.LoginTimestamp }, "IX_LoginHistory_UserId_LoginTimestamp").IsDescending(false, true);

            entity.HasIndex(e => e.UsernameAttempt, "IX_LoginHistory_UsernameAttempt");

            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.LoginTimestamp).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserAgent).HasMaxLength(512);
            entity.Property(e => e.UsernameAttempt).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.LoginHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LoginHistory_Users");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E3996B21932");

            entity.HasIndex(e => e.Token, "UQ__RefreshT__1EB4F817A9CBB3DC").IsUnique();

            entity.Property(e => e.Token).HasMaxLength(256);
            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A7E5E6282");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160B4BAE49A").IsUnique();

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RoleName).HasMaxLength(256);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5E086730");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41D656D83").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534EF91F56A").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A35F36F77A9");

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

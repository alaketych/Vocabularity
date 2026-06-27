using Microsoft.EntityFrameworkCore;
using Vocabularity.Core.Entities;

namespace Vocabularity.Core.Data;

public class VocabularityDbContext : DbContext
{
    public VocabularityDbContext(DbContextOptions<VocabularityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Language> Languages => Set<Language>();

    public DbSet<DictionaryEntry> Dictionaries => Set<DictionaryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.UserLogin).HasColumnName("user_login").HasMaxLength(256);
            entity.Property(e => e.Pseudonym).HasColumnName("user_pseudonym").HasMaxLength(256);
            entity.Property(e => e.Password).HasColumnName("user_password").HasMaxLength(512);
            entity.Property(e => e.PasswordSalt).HasColumnName("user_password_salt").HasMaxLength(512);
            entity.Property(e => e.Ttl).HasColumnName("ttl");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Languages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Name).HasColumnName("language_name").HasMaxLength(256);
            entity.Property(e => e.LanguageImage).HasColumnName("language_image").HasMaxLength(1024);
            entity.Property(e => e.Ttl).HasColumnName("ttl");
        });

        modelBuilder.Entity<DictionaryEntry>(entity =>
        {
            entity.ToTable("Dictionaries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Name).HasColumnName("Dictionary_name").HasMaxLength(256);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(36);
            entity.Property(e => e.LanguageId).HasColumnName("language_id").HasMaxLength(36);
            entity.Property(e => e.OriginalWord).HasColumnName("original_word").HasMaxLength(512);
            entity.Property(e => e.OriginalTranscriptionedWord)
                .HasColumnName("original_transcriptioned_word")
                .HasMaxLength(512);
            entity.Property(e => e.TranslatedWord).HasColumnName("translated_word").HasMaxLength(512);
            entity.Property(e => e.Ttl).HasColumnName("ttl");
        });
    }
}

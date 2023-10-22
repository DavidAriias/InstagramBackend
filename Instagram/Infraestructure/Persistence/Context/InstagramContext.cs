using System;
using System.Collections.Generic;
using Instagram.Infraestructure.Data.Models.SQL;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Infraestructure.Persistence.Context;

public partial class InstagramContext : DbContext
{
    public InstagramContext()
    {
    }

    public InstagramContext(DbContextOptions<InstagramContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Birthday> Birthdays { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<UserDatum> UserData { get; set; }

    public virtual DbSet<UserDescription> UserDescriptions { get; set; }

    public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }

    public virtual DbSet<UserLink> UserLinks { get; set; }

    public virtual DbSet<UserName> UserNames { get; set; }

    public DbSet<UserStoredProcedure> UserStoredProcedures { get; set; }

    public DbSet<SearchStoredProcedure> SearchStoredProcedures { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<UserStoredProcedure>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<SearchStoredProcedure>(entity =>
        {
            entity.HasNoKey();
        });


        modelBuilder.Entity<Birthday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("birthday_pkey");

            entity.ToTable("birthday");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Birthdaydate).HasColumnName("birthdaydate");
            entity.Property(e => e.Updatedate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedate");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Birthdays)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_token_pkey");

            entity.ToTable("refresh_token");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.TokenValue)
                .HasMaxLength(255)
                .HasColumnName("token_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<UserDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_data_pkey");

            entity.ToTable("user_data");

            entity.HasIndex(e => e.Email, "user_data_email_key").IsUnique();

            entity.HasIndex(e => e.Phonenumber, "user_data_phonenumber_key").IsUnique();

            entity.HasIndex(e => e.Username, "user_data_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserDescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_description_pkey");

            entity.ToTable("user_description");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.Imageprofile)
                .HasMaxLength(300)
                .HasColumnName("imageprofile");
            entity.Property(e => e.Isprivated).HasColumnName("isprivated");
            entity.Property(e => e.Isverificated).HasColumnName("isverificated");
            entity.Property(e => e.Pronoun)
                .HasMaxLength(10)
                .HasColumnName("pronoun");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserDescriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<UserDeviceToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_device_token_pkey");

            entity.ToTable("user_device_token");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceToken)
                .HasMaxLength(255)
                .HasColumnName("device_token");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(20)
                .HasColumnName("device_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserDeviceTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<UserLink>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("user_link");

            entity.Property(e => e.Link)
                .HasMaxLength(200)
                .HasColumnName("link");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<UserName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_name_pkey");

            entity.ToTable("user_name");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Updatedate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedate");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserNames)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HuobiProject.Models
{
    public partial class HuobiContext : DbContext
    {
        public HuobiContext(DbContextOptions<HuobiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TArticle> TArticle { get; set; }
        public virtual DbSet<TArticleLang> TArticleLang { get; set; }
        public virtual DbSet<TAttention> TAttention { get; set; }
        public virtual DbSet<TComment> TComment { get; set; }
        public virtual DbSet<TCommentLike> TCommentLike { get; set; }
        public virtual DbSet<THelpful> THelpful { get; set; }
        public virtual DbSet<TLang> TLang { get; set; }
        public virtual DbSet<TResource> TResource { get; set; }
        public virtual DbSet<TTicket> TTicket { get; set; }
        public virtual DbSet<TTicketChat> TTicketChat { get; set; }
        public virtual DbSet<TType> TType { get; set; }
        public virtual DbSet<TUser> TUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<TArticle>(entity =>
            {
                entity.HasKey(e => e.ArticleId);

                entity.ToTable("T_Article", "Huobi");

                entity.Property(e => e.ArticleId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<TArticleLang>(entity =>
            {
                entity.HasKey(e => e.ArticleLangId);

                entity.ToTable("T_ArticleLang", "Huobi");

                entity.HasIndex(e => e.ArticleId)
                    .HasName("T_ArticleLang_ibfk_5");

                entity.HasIndex(e => e.TypeId)
                    .HasName("TypeId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.ArticleLangId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Commentable)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Flag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TypeId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TArticleLang)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("T_ArticleLang_ibfk_5");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TArticleLang)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_ArticleLang_ibfk_3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TArticleLang)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_ArticleLang_ibfk_4");
            });

            modelBuilder.Entity<TAttention>(entity =>
            {
                entity.HasKey(e => e.AttentionId);

                entity.ToTable("T_Attention", "Huobi");

                entity.HasIndex(e => e.TypeId)
                    .HasName("T_Attention_ibfk_2");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.AttentionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.TypeId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TAttention)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("T_Attention_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TAttention)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("T_Attention_ibfk_1");
            });

            modelBuilder.Entity<TComment>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("T_Comment", "Huobi");

                entity.HasIndex(e => e.ArticleLangId)
                    .HasName("T_Comment_ibfk_4");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ArticleLangId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ArticleLang)
                    .WithMany(p => p.TComment)
                    .HasForeignKey(d => d.ArticleLangId)
                    .HasConstraintName("T_Comment_ibfk_4");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TComment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_Comment_ibfk_3");
            });

            modelBuilder.Entity<TCommentLike>(entity =>
            {
                entity.HasKey(e => e.CommentLikeId);

                entity.ToTable("T_CommentLike", "Huobi");

                entity.HasIndex(e => e.CommentId)
                    .HasName("CommentId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.CommentLikeId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CommentId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Like).HasColumnType("bit(1)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.TCommentLike)
                    .HasForeignKey(d => d.CommentId)
                    .HasConstraintName("T_CommentLike_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TCommentLike)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_CommentLike_ibfk_1");
            });

            modelBuilder.Entity<THelpful>(entity =>
            {
                entity.HasKey(e => e.HelpfulId);

                entity.ToTable("T_Helpful", "Huobi");

                entity.HasIndex(e => e.ArticleLangId)
                    .HasName("T_Helpful_ibfk_1");

                entity.Property(e => e.HelpfulId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ArticleLangId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Helpful).HasColumnType("bit(1)");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.ArticleLang)
                    .WithMany(p => p.THelpful)
                    .HasForeignKey(d => d.ArticleLangId)
                    .HasConstraintName("T_Helpful_ibfk_1");
            });

            modelBuilder.Entity<TLang>(entity =>
            {
                entity.HasKey(e => e.LangId);

                entity.ToTable("T_Lang", "Huobi");

                entity.Property(e => e.LangId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TResource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.ToTable("T_Resource", "Huobi");

                entity.Property(e => e.ResourceId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TypeId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TTicket>(entity =>
            {
                entity.HasKey(e => e.TicketId);

                entity.ToTable("T_Ticket", "Huobi");

                entity.HasIndex(e => e.StatusId)
                    .HasName("StatusId");

                entity.HasIndex(e => e.TypeId)
                    .HasName("TypeId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.TicketId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StatusId)
                    .HasColumnType("int(11) unsigned")
                    .HasDefaultValueSql("700001");

                entity.Property(e => e.TypeId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TTicketStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_Ticket_ibfk_2");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TTicketType)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_Ticket_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TTicket)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("T_Ticket_ibfk_3");
            });

            modelBuilder.Entity<TTicketChat>(entity =>
            {
                entity.HasKey(e => e.TicketChatId);

                entity.ToTable("T_TicketChat", "Huobi");

                entity.HasIndex(e => e.Annex)
                    .HasName("Annex");

                entity.HasIndex(e => e.TicketId)
                    .HasName("TicketId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.TicketChatId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Annex)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.TicketId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AnnexNavigation)
                    .WithMany(p => p.TTicketChat)
                    .HasForeignKey(d => d.Annex)
                    .HasConstraintName("T_TicketChat_ibfk_3");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TTicketChat)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("T_TicketChat_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TTicketChat)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("T_TicketChat_ibfk_2");
            });

            modelBuilder.Entity<TType>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("T_Type", "Huobi");

                entity.HasIndex(e => e.LangId)
                    .HasName("LangId");

                entity.HasIndex(e => e.ParentId)
                    .HasName("ParentId");

                entity.Property(e => e.TypeId)
                    .HasColumnType("int(11) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LangId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Lang)
                    .WithMany(p => p.TType)
                    .HasForeignKey(d => d.LangId)
                    .HasConstraintName("T_Type_ibfk_2");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("T_Type_ibfk_1");
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("T_User", "Huobi");

                entity.HasIndex(e => e.Account)
                    .HasName("Account")
                    .IsUnique();

                entity.HasIndex(e => e.Photo)
                    .HasName("Photo");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Photo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("1000011562032294099-8287");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Super)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PhotoNavigation)
                    .WithMany(p => p.TUser)
                    .HasForeignKey(d => d.Photo)
                    .HasConstraintName("T_User_ibfk_1");
            });
        }
    }
}

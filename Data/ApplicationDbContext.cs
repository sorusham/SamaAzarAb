using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MessageForAzarab.Models;

namespace MessageForAzarab.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Letter> Letters { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<AttachmentFile> AttachmentFiles { get; set; }
        public DbSet<AttachmentRevision> AttachmentRevisions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        // اضافه کردن مدل‌های جدید سیستم DCC
        public DbSet<BaseDocument> BaseDocuments { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentTransaction> DocumentTransactions { get; set; }
        public DbSet<DocumentAttachment> DocumentAttachments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Letter>()
                .HasMany(l => l.Attachments)
                .WithOne(a => a.Letter)
                .HasForeignKey(a => a.LetterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attachment>()
                .HasMany(a => a.Files)
                .WithOne(f => f.Attachment)
                .HasForeignKey(f => f.AttachmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attachment>()
                .HasMany(a => a.Revisions)
                .WithOne(r => r.Attachment)
                .HasForeignKey(r => r.AttachmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.SentLetters)
                .WithOne(l => l.Sender)
                .HasForeignKey(l => l.SenderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedLetters)
                .WithOne(l => l.Receiver)
                .HasForeignKey(l => l.ReceiverId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.SentAttachments)
                .WithOne(a => a.Sender)
                .HasForeignKey(a => a.SenderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedAttachments)
                .WithOne(a => a.Receiver)
                .HasForeignKey(a => a.ReceiverId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.SentRevisions)
                .WithOne(r => r.Sender)
                .HasForeignKey(r => r.SenderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedRevisions)
                .WithOne(r => r.Receiver)
                .HasForeignKey(r => r.ReceiverId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
                
            // تنظیمات جدید برای سیستم DCC
            
            // ارتباط بین BaseDocument و DocumentVersion
            builder.Entity<BaseDocument>()
                .HasMany(d => d.DocumentVersions)
                .WithOne(v => v.BaseDocument)
                .HasForeignKey(v => v.BaseDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // ارتباط بین DocumentVersion و DocumentTransaction
            builder.Entity<DocumentVersion>()
                .HasMany(v => v.Transactions)
                .WithOne(t => t.DocumentVersion)
                .HasForeignKey(t => t.DocumentVersionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // ارتباط بین DocumentVersion و DocumentAttachment
            builder.Entity<DocumentVersion>()
                .HasMany(v => v.Attachments)
                .WithOne(a => a.DocumentVersion)
                .HasForeignKey(a => a.DocumentVersionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // ارتباط بین Department و BaseDocument
            builder.Entity<Department>()
                .HasMany(d => d.Documents)
                .WithOne(doc => doc.Department)
                .HasForeignKey(doc => doc.DepartmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            // ارتباط بین Department و Project
            builder.Entity<Department>()
                .HasMany(d => d.Projects)
                .WithOne(p => p.Department)
                .HasForeignKey(p => p.DepartmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
                
            // ارتباط بین UserDepartment با ApplicationUser و Department
            builder.Entity<UserDepartment>()
                .HasOne(ud => ud.User)
                .WithMany(u => u.UserDepartments)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<UserDepartment>()
                .HasOne(ud => ud.Department)
                .WithMany(d => d.UserDepartments)
                .HasForeignKey(ud => ud.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // ارتباط مدورانه بین DocumentVersion با self (نسخه قبلی)
            builder.Entity<DocumentVersion>()
                .HasOne(v => v.PreviousVersion)
                .WithMany()
                .HasForeignKey(v => v.PreviousVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

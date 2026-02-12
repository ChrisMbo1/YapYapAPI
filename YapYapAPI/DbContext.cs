using Microsoft.EntityFrameworkCore;
using YapYapAPI.Models;

namespace YapYapAPI.Data
{
    public class YapYapDbContext : DbContext
    {
        public YapYapDbContext(DbContextOptions<YapYapDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupInvite> GroupInvites { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
{
    entity.ToTable("users");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.Name).HasColumnName("username").IsRequired();
    entity.Property(e => e.Password).HasColumnName("password").IsRequired();
    entity.Property(e => e.BIO).HasColumnName("bio");
    entity.Property(e => e.status_id).HasColumnName("status_id");
    entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

    entity.HasOne(u => u.Status)
    .WithMany(s => s.Users)
    .HasForeignKey(u => u.status_id)
    .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<Status>(entity =>
{
    entity.ToTable("status");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.StatusType).HasColumnName("status_type").IsRequired();
});

            modelBuilder.Entity<FriendRequest>(entity =>
{
    entity.ToTable("friend_requests");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.SenderId).HasColumnName("sender_id");
    entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
    entity.Property(e => e.Status).HasColumnName("status").IsRequired();
    entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

    entity.HasOne(fr => fr.Sender)
    .WithMany()
    .HasForeignKey(fr => fr.SenderId)
    .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(fr => fr.Receiver)
        .WithMany()
        .HasForeignKey(fr => fr.ReceiverId)
        .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<Friend>(entity =>
{
    entity.ToTable("friends");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.UserOne).HasColumnName("user_one");
    entity.Property(e => e.UserTwo).HasColumnName("user_two");

    entity.HasOne(f => f.UserOneNavigation)
    .WithMany()
    .HasForeignKey(f => f.UserOne)
    .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(f => f.UserTwoNavigation)
        .WithMany()
        .HasForeignKey(f => f.UserTwo)
        .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<Group>(entity =>
{
    entity.ToTable("groups");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.AdminId).HasColumnName("admin_id");
    entity.Property(e => e.Name).HasColumnName("name").IsRequired();
    entity.Property(e => e.Description).HasColumnName("description");
    entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

    entity.HasOne(g => g.Admin)
    .WithMany()
    .HasForeignKey(g => g.AdminId)
    .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<GroupInvite>(entity =>
{
    entity.ToTable("group_invite");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.SenderId).HasColumnName("sender_id");
    entity.Property(e => e.GroupId).HasColumnName("group_id");

    entity.HasOne(gi => gi.Sender)
    .WithMany()
    .HasForeignKey(gi => gi.SenderId)
    .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(gi => gi.Group)
        .WithMany(g => g.GroupInvites)
        .HasForeignKey(gi => gi.GroupId)
        .OnDelete(DeleteBehavior.Cascade);
});

            modelBuilder.Entity<UserGroup>(entity =>
{
    entity.ToTable("user_groups");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.GroupId).HasColumnName("group_id");
    entity.Property(e => e.UserId).HasColumnName("user_id");
    entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

    entity.HasOne(ug => ug.Group)
    .WithMany(g => g.UserGroups)
    .HasForeignKey(ug => ug.GroupId)
    .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(ug => ug.User)
        .WithMany()
        .HasForeignKey(ug => ug.UserId)
        .OnDelete(DeleteBehavior.Cascade);
});

            modelBuilder.Entity<Chat>(entity =>
{
    entity.ToTable("chats");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.UserOne).HasColumnName("user_one");
    entity.Property(e => e.UserTwo).HasColumnName("user_two");

    entity.HasOne(c => c.UserOneNavigation)
    .WithMany()
    .HasForeignKey(c => c.UserOne)
    .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(c => c.UserTwoNavigation)
        .WithMany()
        .HasForeignKey(c => c.UserTwo)
        .OnDelete(DeleteBehavior.Restrict);
});

            modelBuilder.Entity<ChatMessage>(entity =>
{
    entity.ToTable("chat_message");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.Message).HasColumnName("message").IsRequired();
    entity.Property(e => e.SenderId).HasColumnName("sender_id");
    entity.Property(e => e.ChatId).HasColumnName("chat_id");
    entity.Property(e => e.GroupId).HasColumnName("group_id");
    entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

    entity.HasOne(cm => cm.Sender)
    .WithMany()
    .HasForeignKey(cm => cm.SenderId)
    .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(cm => cm.Chat)
        .WithMany(c => c.ChatMessages)
        .HasForeignKey(cm => cm.ChatId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(cm => cm.Group)
        .WithMany(g => g.ChatMessages)
        .HasForeignKey(cm => cm.GroupId)
        .OnDelete(DeleteBehavior.Cascade);
});

            modelBuilder.Entity<Status>().HasData(
    new Status { Id = 1, StatusType = "Online" },
    new Status { Id = 2, StatusType = "Away" },
    new Status { Id = 3, StatusType = "Busy" },
    new Status { Id = 4, StatusType = "Offline" }
);
        }
    }
}
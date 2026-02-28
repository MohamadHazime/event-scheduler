using EventScheduler.Domain.EventAttendees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventScheduler.Infrastructure.Data.Configurations;

public class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
{
    public void Configure(EntityTypeBuilder<EventAttendee> builder)
    {
        builder.HasKey(ea => ea.Id);

        builder.HasOne(ea => ea.Event)
            .WithMany(e => e.Attendees)
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.User)
            .WithMany(u => u.Attendances)
            .HasForeignKey(ea => ea.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ea => new { ea.EventId, ea.UserId })
            .IsUnique();

        builder.Property(ea => ea.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
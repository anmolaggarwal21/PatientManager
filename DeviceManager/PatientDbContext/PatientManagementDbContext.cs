using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DeviceManager
{
    public class PatientManagementDbContext: DbContext
    {
        public PatientManagementDbContext(DbContextOptions<PatientManagementDbContext> contextOptions) : base(contextOptions)
        {
        }

        public DbSet<ProviderEntity> ProviderEntities => Set<ProviderEntity>();
        public DbSet<PatientEntity> PatientEntities => Set<PatientEntity>();
		public DbSet<ClaimEntity> ClaimEntities => Set<ClaimEntity>();
        public DbSet<BillingEntity> BillingEntities => Set<BillingEntity>();
        protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<PatientEntity>().
				Property(p => p.DOB)
				.HasColumnType("date");

			builder.Entity<PatientEntity>().
				Property(p => p.AdmissionDate)
				.HasColumnType("date");

			builder.Entity<ClaimEntity>().
				HasOne(x=> x.Patient)
				.WithMany()
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ClaimEntity>().
			HasOne(x => x.Provider)
			.WithMany()
			.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<BillingEntity>().
				Property(x => x.AdmissionDate)
				.HasColumnType("date");

			builder.Entity<BillingEntity>()
				.OwnsOne(x => x.BillingItem, bi => { 
					bi.ToJson();
					bi.OwnsOne(x => x.POSBillingItem, y => {
						y.Property(z => z.Type);
						y.OwnsMany(z => z.Data);
					});
					bi.OwnsMany(x => x.OtherBillingItems, y => {
						y.Property(z => z.Type);
						y.OwnsMany(z => z.Data);
					});

				});




		}
	}
}

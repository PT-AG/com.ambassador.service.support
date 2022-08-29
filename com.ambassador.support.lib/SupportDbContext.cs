using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace com.ambassador.support.lib
{
    public class SupportDbContext : DbContext
    {
        public virtual DbSet<BEACUKAI_TEMP> BeacukaiTemp { get; set; }
        public virtual DbSet<FactBeacukai> FactBeacukai { get; set; }
		public virtual DbSet<ScrapViewModel> ViewScrap{ get; set; }
        public virtual DbSet<ViewFactBeacukai> ViewFactBeacukai { get; set; }
		public virtual DbSet<WIPViewModel> ViewWIP { get; set; }
        public virtual DbSet<HOrder> HOrder { get; set; }

        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BEACUKAI_TEMP>(entity =>
            {
                entity.ToTable("BEACUKAI_TEMP");

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Barang)
                    .HasMaxLength(100)
                    .IsUnicode(false);

				entity.Property(e => e.Tipe)
				   .HasMaxLength(100)
				   .IsUnicode(false);

				entity.Property(e => e.BCId)
                    .HasColumnName("BCId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BCNo)
                    .HasColumnName("BCNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Bruto).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.CIF)
                    .HasColumnName("CIF")
                    .HasColumnType("decimal(20, 2)");

                entity.Property(e => e.CIF_Rupiah)
                    .HasColumnName("CIF_Rupiah")
                    .HasColumnType("decimal(20, 2)");

                entity.Property(e => e.Hari).HasColumnType("datetime");

                entity.Property(e => e.IDHeader).HasColumnName("IDHeader");

                entity.Property(e => e.JenisBC)
                    .HasColumnName("JenisBC")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.JenisDokumen)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.JumlahSatBarang).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.KodeBarang)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KodeDokumenPabean)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KodeKemasan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KodeSupplier)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NamaKemasan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NamaSupplier)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Netto).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.NoAju)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NomorDokumen)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sat)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.TanggalDokumen).HasColumnType("datetime");

                entity.Property(e => e.TglBCNo)
                    .HasColumnName("TglBCNo")
                    .HasColumnType("datetime");

                entity.Property(e => e.TglDaftarAju).HasColumnType("datetime");

                entity.Property(e => e.TglDatang).HasColumnType("datetime");

                entity.Property(e => e.Valuta)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Vendor)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FactBeacukai>(entity =>
            {
                entity.HasKey(e => e.DetailshippingOrderId);

                entity.Property(e => e.DetailshippingOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.BCDate)
                    .HasColumnName("BCDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.BCId)
                    .HasColumnName("BCId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BCNo)
                    .HasColumnName("BCNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BCType)
                    .HasColumnName("BCType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BonDate).HasColumnType("datetime");

                entity.Property(e => e.BonNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nominal).HasColumnType("money");

                entity.Property(e => e.PricePo).HasColumnType("money");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.SupllierCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitQtyName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

			modelBuilder.Entity<ScrapViewModel>(entity =>
			{
				 
				entity.Property(e => e.ClassificationId)
					.HasColumnName("ClassificationId");

				entity.Property(e => e.ClassificationId);
				 

				entity.Property(e => e.ClassificationCode);

				entity.Property(e => e.ClassificationName);

				entity.Property(e => e.DestinationId);

				entity.Property(e => e.UnitQtyName);

				entity.Property(e => e.SaldoAwal) ;

				entity.Property(e => e.Pemasukan) ;

				entity.Property(e => e.Pengeluaran);

				entity.Property(e => e.Penyesuaian) ;

				entity.Property(e => e.StockOpname);

				entity.Property(e => e.Selisih);

				entity.Property(e => e.StockId);

				entity.Property(e => e.SaldoBuku);

			
			});

			modelBuilder.Entity<WIPViewModel>(entity =>
			{

				entity.Property(e => e.Kode);
				entity.Property(e => e.Comodity);
				entity.Property(e => e.WIP);
				entity.Property(e => e.UnitQtyName );

			});
		}
    }
}


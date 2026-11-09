using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Maros.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Persistence;

public static class TestimonialsSeed
{
    public static async Task SeedDefaultAsync(MarosDbContext context)
    {
        // Si ya existen suficientes testimonios cargados, no duplicamos
        if (await context.Testimonials.CountAsync() >= 10) return;

        var sampleTestimonials = new List<Testimonial>
        {
            new()
            {
                ClientName = "Camila Restrepo",
                City = "Medellín",
                Rating = 5,
                Quote = "Quedé enamorada desde que abrí la caja. La envoltura tan delicada, el aroma delicioso que trae la prenda y el lazo de regalo demuestran el amor con el que hacen todo. La tela es increíblemente suave y fresca.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                ClientName = "Valentina Gómez",
                City = "Bogotá",
                Rating = 5,
                Quote = "La calidad de las costuras y los terminados es impecable. Se nota que es confección colombiana hecha a mano de verdad. No se motosea ni pierde color tras varias lavadas. ¡100% recomendada!",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                ClientName = "María Alejandra Morales",
                City = "Cali",
                Rating = 5,
                Quote = "Pedí el set de parejas para nuestro aniversario y fue un acierto total. La asesoría por WhatsApp para elegir las tallas de ambos fue súper atenta y acertada. A mi esposo le encantó la comodidad del pantalón.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-8)
            },
            new()
            {
                ClientName = "Carolina Henao",
                City = "Bucaramanga",
                Rating = 5,
                Quote = "Tenía miedo de que no llegara a tiempo para el cumpleaños de mi mamá, pero el envío fue súper rápido a Bucaramanga. Llegó antes de lo esperado y en perfecto estado. ¡Mil gracias por la gestión tan eficiente!",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-12)
            },
            new()
            {
                ClientName = "Daniela Pardo",
                City = "Barranquilla",
                Rating = 5,
                Quote = "Vivo en clima caliente y me daba miedo el calor, pero la tela es ultra fresca y transpirable. El diseño de short y tiras es comodísimo y súper femenino. Ya encargué mi segunda pijama.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                ClientName = "Andrés Felipe Caicedo",
                City = "Manizales",
                Rating = 5,
                Quote = "La atención al cliente es de otro nivel. Me ayudaron a elegir el diseño perfecto para sorprender a mi novia, resolvieron todas mis dudas con paciencia y le añadieron una dedicatoria hermosa. ¡Ella quedó fascinada!",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-18)
            },
            new()
            {
                ClientName = "Paola Andrea Duque",
                City = "Pereira",
                Rating = 5,
                Quote = "Mandé a personalizar mi pijama con las iniciales bordadas y el resultado superó mis expectativas. El bordado es fino, limpio y no pica por dentro. Una prenda de lujo a un precio muy justo.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-22)
            },
            new()
            {
                ClientName = "Sofía Castaño",
                City = "Cartagena",
                Rating = 5,
                Quote = "Hicimos el pedido familiar para Navidad (papá, mamá y nuestros dos niños) y las fotos quedaron soñadas. Las tallas de los niños fueron exactas y los estampados son preciosos.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-26)
            },
            new()
            {
                ClientName = "Laura Victoria Rincón",
                City = "Ibagué",
                Rating = 5,
                Quote = "La tela se siente como una caricia en la piel. Es la pijama más cómoda que he tenido en mi vida; perfecta tanto para descansar los fines de semana como para trabajar cómoda desde casa.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-30)
            },
            new()
            {
                ClientName = "Juliana Suescún",
                City = "Santa Marta",
                Rating = 5,
                Quote = "El unboxing fue toda una experiencia sensorial. El empaque boutique con papel de seda, la tarjetita escrita con cariño y la pijama impecablemente planchada. Cuidan cada detalle al máximo.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-35)
            },
            new()
            {
                ClientName = "Felipe & Mariana",
                City = "Envigado",
                Rating = 5,
                Quote = "Llevamos comprándole pijamas a Maro's desde hace dos años y la calidad siempre es constante y excelente. La tela de algodón conserva su elasticidad y suavidad intactas.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-40)
            },
            new()
            {
                ClientName = "Natalia Zuluaga",
                City = "Cúcuta",
                Rating = 5,
                Quote = "Soy talla XL y siempre me costaba encontrar pijamas con buen corte y que no quedaran apretadas. En Maro's el tallaje es real, holgado y elegante. Me siento hermosa y cómoda.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-45)
            },
            new()
            {
                ClientName = "Diana Marcela Ortiz",
                City = "Villavicencio",
                Rating = 5,
                Quote = "Tuve una duda sobre los colores de la tela y me mandaron fotos reales y videos de cómo se veía a la luz. Esa transparencia y amabilidad no se encuentra en cualquier tienda online.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-50)
            },
            new()
            {
                ClientName = "Luisa Fernanda Mejía",
                City = "Armenia",
                Rating = 5,
                Quote = "Compré la bata para el día de mi boda y fue el complemento perfecto para las fotos del arreglo. Elegante, con una caída de ensueño y súper suave. Quedé sumamente agradecida.",
                Status = TestimonialStatus.Publicado,
                PublishDate = DateTime.UtcNow.AddDays(-55)
            }
        };

        // Si ya hay 1 existente (el de Yesmel), agregamos los que no tengan el mismo nombre
        foreach (var t in sampleTestimonials)
        {
            var exists = await context.Testimonials.AnyAsync(existing => existing.ClientName == t.ClientName);
            if (!exists)
            {
                await context.Testimonials.AddAsync(t);
            }
        }

        await context.SaveChangesAsync();
    }
}


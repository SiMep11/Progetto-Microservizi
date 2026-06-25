using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magazzino.Business.Abstraction; // Dove hai messo IProdottiManager
using Magazzino.Repository.Abstraction; // Dove hai messo IMagazzinoRepository
using Magazzino.Shared; // Dove si trova ProdottoDto

namespace Magazzino.Business
{
    public class ProdottiManager(IMagazzinoRepository repository) : IProdottiManager
    {
        public async Task<IEnumerable<ProdottoDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // Passiamo il token al repository
            var prodottiDb = await repository.GetAllProdottiAsync(cancellationToken);

            return prodottiDb.Select(p => new ProdottoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Descrizione = p.Descrizione,
                Prezzo = p.Prezzo,
                QuantitaDisponibile = p.QuantitaInMagazzino
            });
        }

        public async Task<ProdottoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Passiamo il token al repository
            var prodottoDb = await repository.GetProdottoByIdAsync(id, cancellationToken);

            if (prodottoDb == null)
            {
                return null;
            }

            return new ProdottoDto
            {
                Id = prodottoDb.Id,
                Nome = prodottoDb.Nome,
                Descrizione = prodottoDb.Descrizione,
                Prezzo = prodottoDb.Prezzo,
                QuantitaDisponibile = prodottoDb.QuantitaInMagazzino
            };
        }

        public async Task<ProdottoDto> CreaProdottoAsync(ProdottoDto prodottoDto, CancellationToken cancellationToken = default)
        {
            var nuovoProdotto = new Prodotto
            {
                Nome = prodottoDto.Nome,
                Descrizione = prodottoDto.Descrizione,
                Prezzo = prodottoDto.Prezzo,
                QuantitaInMagazzino = prodottoDto.QuantitaDisponibile
            };

            // 2. Diciamo al DbContext di tracciare il nuovo prodotto in memoria
            await repository.CreaProdottoAsync(nuovoProdotto, cancellationToken);

            // 3. IL TOCCO DEL PROF: Il Business comanda il salvataggio fisico!
            await repository.SaveChangesAsync(cancellationToken);

            // 4. Magia di EF Core: dopo il SaveChanges, 'nuovoProdotto' ha l'ID valorizzato!
            prodottoDto.Id = nuovoProdotto.Id;

            return prodottoDto;
        }

        public async Task ScaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default)
        {
            // 1. Il repository calcola la nuova giacenza in memoria
            await repository.ScaricaGiacenzaAsync(id, quantita, cancellationToken);

            // 2. IL TOCCO DEL PROF: Il Business comanda il salvataggio fisico!
            await repository.SaveChangesAsync(cancellationToken);
        }
    }

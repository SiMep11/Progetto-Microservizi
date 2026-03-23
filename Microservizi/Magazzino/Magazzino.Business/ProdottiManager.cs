using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magazzino.Business.Abstraction; // Dove hai messo IProdottiManager
using Magazzino.Repository.Abstraction; // Dove hai messo IMagazzinoRepository
using Magazzino.Shared; // Dove si trova ProdottoDto

namespace Magazzino.Business
{
    public class ProdottiManager : IProdottiManager
    {
        // Ora usiamo il Repository, non più il DbContext direttamente!
        private readonly IMagazzinoRepository _repository;

        // Dependency Injection
        public ProdottiManager(IMagazzinoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProdottoDto>> GetAllAsync()
        {
            // 1. Chiediamo le Entities (Tabelle) al Repository
            var prodottiDb = await _repository.GetAllProdottiAsync();

            // 2. Mapping: Entity -> DTO
            return prodottiDb.Select(p => new ProdottoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Descrizione = p.Descrizione,
                Prezzo = p.Prezzo,
                QuantitaDisponibile = p.QuantitaInMagazzino
            });
        }

        public async Task<ProdottoDto?> GetByIdAsync(int id)
        {
            // 1. Chiediamo la singola Entity al Repository
            var prodottoDb = await _repository.GetProdottoByIdAsync(id);

            // Se non esiste, restituiamo null
            if (prodottoDb == null)
            {
                return null;
            }

            // 2. Mapping: Entity -> DTO
            return new ProdottoDto
            {
                Id = prodottoDb.Id,
                Nome = prodottoDb.Nome,
                Descrizione = prodottoDb.Descrizione,
                Prezzo = prodottoDb.Prezzo,
                QuantitaDisponibile = prodottoDb.QuantitaInMagazzino
            };
        }

        public async Task<ProdottoDto> CreaProdottoAsync(ProdottoDto prodottoDto)
        {
            // Trasformiamo il DTO di internet nell'Entity del DB
            var nuovoProdotto = new Magazzino.Repository.Models.Prodotto
            {
                Nome = prodottoDto.Nome,
                Descrizione = prodottoDto.Descrizione,
                Prezzo = prodottoDto.Prezzo,
                QuantitaInMagazzino = prodottoDto.QuantitaDisponibile
            };

            var prodottoSalvato = await _repository.CreaProdottoAsync(nuovoProdotto);

            // Aggiorniamo l'ID del DTO con quello generato dal DB
            prodottoDto.Id = prodottoSalvato.Id;
            return prodottoDto;
        }
    }
}

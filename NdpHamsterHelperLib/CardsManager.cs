using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdpHamsterHelperLib
{
    public class CardsManager
    {
        private List<Card> cards;

        public CardsManager(IEnumerable<Card> cards)
        {
            this.cards = cards.ToList();
        }

        public List<Card>OrderByPayback(IEnumerable<Card> cards)
        {
            return cards.OrderBy(c => c.Payback).ToList();
        }

        public void BuyCard(Card card)
        {
            File.Delete(card.FilePath);
            cards.Remove(card);
        }
    }
}

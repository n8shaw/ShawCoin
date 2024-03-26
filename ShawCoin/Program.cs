using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using EllipticCurve;
using Newtonsoft.Json;

namespace ShawCoin
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            PrivateKey key3 = new PrivateKey();
            PublicKey wallet3 = key3.publicKey();

            Blockchain shawCoin = new Blockchain(2, 100);

            shawCoin.MinePendingTransactions(wallet1);
            shawCoin.MinePendingTransactions(wallet2);
            shawCoin.MinePendingTransactions(wallet3);

            Console.Write("\nBalance of Wallet1: $" + shawCoin.GetBalanceOfWallet(wallet1));
            Console.Write("\nBalance of Wallet2: $" + shawCoin.GetBalanceOfWallet(wallet2));
            Console.Write("\nBalance of Wallet3: $" + shawCoin.GetBalanceOfWallet(wallet3));

            Transaction tx1 = new Transaction(wallet1, wallet2, 55.0m);
            tx1.SignTransaction(key1);
            shawCoin.AddPendingTransaction(tx1);

            Transaction tx2 = new Transaction(wallet3, wallet2, 20.0m);
            tx2.SignTransaction(key3);
            shawCoin.AddPendingTransaction(tx2);

            shawCoin.MinePendingTransactions(wallet3);

            Console.Write("\nBalance of Wallet1: $" + shawCoin.GetBalanceOfWallet(wallet1));
            Console.Write("\nBalance of Wallet2: $" + shawCoin.GetBalanceOfWallet(wallet2));
            Console.Write("\nBalance of Wallet3: $" + shawCoin.GetBalanceOfWallet(wallet3));





            string blockJson = JsonConvert.SerializeObject(shawCoin, Formatting.Indented);

            Console.WriteLine(blockJson);

            if (shawCoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is Valid");
            }
            else
            {
                Console.WriteLine("Blockcahin is NOT valid");
            }
        }
    }

} 



using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EllipticCurve;

namespace ShawCoin
{
    internal class Blockchain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> PendingTransactions { get; set; }
        public decimal MiningReward { get; set; }
        public Blockchain(int difficulty, decimal miningReward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = miningReward;
            this.PendingTransactions = new List<Transaction>();
        }

        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyMMddHHmmssfff"), new List<Transaction>());
        }

        public Block GetLastesBlock()
        {
            return this.Chain.Last();
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLastesBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);

        }
        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block currentBlock = this.Chain[i];
                Block previosBlock = this.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
                if (currentBlock.PreviousHash != previosBlock.Hash)
                {
                    return false;
                }

            }
            return true;
        }

        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;
            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if(!(transaction.FromAddress is null))
                    {
                        string fromAddressDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");

                        if (fromAddressDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                    
                    string toAddressDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");

                    if (toAddressDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        public void AddPendingTransaction(Transaction tx)
        {
            if (tx.ToAddress is null)
            {
                throw new Exception("Transactions must have a to address");
            }

            if (tx.Amount > this.GetBalanceOfWallet(tx.FromAddress))
            {
                throw new Exception("There must be sufficent funds in the wallet");
            }
            if (tx.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block");
            }

            this.PendingTransactions.Add(tx);

        }

        public void MinePendingTransactions(PublicKey publicKey)
        {
            Transaction rewardTx = new Transaction(null, publicKey, this.MiningReward);
            this.PendingTransactions.Add(rewardTx);

            Block newBlock = new Block(GetLastesBlock().Index + 1, DateTime.Now.ToString("yyyMMddHHmmssfff"), this.PendingTransactions, GetLastesBlock().Hash);
            newBlock.Mine(this.Difficulty);
            this.Chain.Add(newBlock);
            this.PendingTransactions = new List<Transaction>();
        }
    }
}

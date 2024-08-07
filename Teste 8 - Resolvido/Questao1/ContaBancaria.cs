using System;
using System.Globalization;

namespace Questao1
{
	class ContaBancaria
	{
		public int NumeroConta { get; }
		public string NomeTitular { get; }
		private double Saldo { get; set; }

		public ContaBancaria(int numeroConta, string nomeTitular, double depositoInicial = 0)
		{
			NumeroConta = numeroConta;
			NomeTitular = nomeTitular;
			Deposito(depositoInicial); 
		}

		public void Deposito(double valor)
		{
			Saldo += valor;			
		}

		public bool Saque(double valor)
		{
			double taxa = 3.50;
			
			Saldo -= valor + taxa;				
			return true;
			
		}

		public override string ToString()
		{
			return $"Conta {NumeroConta}, Titular: {NomeTitular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
		}
	}

}

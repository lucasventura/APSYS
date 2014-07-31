namespace APSYS.Infrastructure.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
	/// Extensões de lista
	/// </summary>
	public static class IListExtensions
	{
		/// <summary>
		/// Insere uma lista de novos itens
		/// </summary>
		/// <typeparam name="T">Tipo do item da coleção</typeparam>
		/// <param name="items">Coleção de <see cref="T"/></param>
		/// <param name="newItems">Itens que serão adicionados</param>
		public static void AddRange<T>(this ICollection<T> items, IEnumerable<T> newItems)
		{
			foreach (T item in newItems)
			{
				items.Add(item);
			}
		}

		/// <summary>
		/// Retorna se existe algum item igual nas listas
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="collection">Coleção original</param>
		/// <returns>Retorno booleano</returns>
		public static bool Any<T>(this ICollection<T> collection)
		{
			return collection.Count > 0;
		}

		/// <summary>
		/// Retorna se existe algum item igual nas listas
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="collection">Coleção original</param>
		/// <param name="items">Itens que serão testados</param>
		/// <returns>Retorno booleano</returns>
		public static bool Any<T>(this ICollection<T> collection, params T[] items)
		{
			return collection.Any(items.Contains);
		}

		/// <summary>
		/// Retorna se existe algum item igual nas listas
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="collection">Coleção original</param>
		/// <param name="items">Itens que serão testados</param>
		/// <returns>Retorno booleano</returns>
		public static bool Any<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			return collection.Any(items.Contains);
		}

		/// <summary>
		/// Retorna se a coleção está vazia
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="collection">Coleção original</param>
		/// <returns>Retorno booleano</returns>
		public static bool IsEmpty<T>(this IEnumerable<T> collection)
		{
			return !collection.Any();
		}

		/// <summary>
		/// Cria uma representação ReadOnly de uma <see cref="IList{T}"/>
		/// </summary>
		/// <param name="from">Lista base</param>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <returns>Lista ReadOnly</returns>
		public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> from)
		{
			return new ReadOnlyCollection<T>(from);
		}

		/// <summary>
		/// Cria um <see cref="HashSet{T}"/> de uma <see cref="IEnumerable{T}"/>
		/// </summary>
		/// <typeparam name="T"> Tipo do parametro </typeparam>
		/// <param name="from">Instãncia de <see cref="IEnumerable{T}"/></param>
		/// <returns>Instãncia de <see cref="HashSet{T}"/></returns>
		public static HashSet<T> AsHashSet<T>(this IEnumerable<T> from)
		{
			return new HashSet<T>(from);
		}

		/// <summary>
		/// Divide uma lista em várias outras de acordo com o tamanho
		/// </summary>
		/// <typeparam name="T">Tipo do item da coleção</typeparam>
		/// <param name="items">Coleção de <see cref="T"/></param>
		/// <param name="qty">Quantidade limite a ser cortada</param>
		/// <returns>Lista de lista de <see cref="T"/></returns>
		public static IList<IList<T>> Split<T>(this IList<T> items, int qty)
		{
			IList<IList<T>> pacote = new List<IList<T>>();

			if (items.Count < qty)
			{
				pacote.Add(items);

				return pacote;
			}

			double partes = Math.Ceiling((double)items.Count / qty);

			for (int i = 0; i < partes; i++)
			{
				pacote.Add(items.Skip(i * qty).Take(qty).ToList());
			}

			return pacote;
		}

		/// <summary>
		/// Obtem os elementos anteriores ao <see cref="atual"/>
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="atual">Elemento atual</param>
		/// <returns>Elementos anteriores</returns>
		public static IEnumerable<T> Anteriores<T>(this IEnumerable<T> lista, T atual) where T : class
		{
			if (lista == null)
			{
				throw new ArgumentNullException("lista");
			}

			if (!lista.Contains(atual))
			{
				throw new InvalidOperationException("O elemento atual não existe na lista");
			}

			return lista.TakeWhile(p => !p.Equals(atual));
		}

		/// <summary>
		/// Obtem os elementos posteriores ao <see cref="atual"/>
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="atual">Elemento atual</param>
		/// <returns>Elementos posteriores</returns>
		public static IEnumerable<T> Posteriores<T>(this IEnumerable<T> lista, T atual) where T : class
		{
			if (lista == null)
			{
				throw new ArgumentNullException("lista");
			}

			if (!lista.Contains(atual))
			{
				throw new InvalidOperationException("O elemento atual não existe na lista");
			}

			return lista.SkipWhile(p => !p.Equals(atual)).Skip(1);
		}

		/// <summary>
		/// Obtem os elementos posteriores ao <see cref="atual"/>
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="atual">Elemento atual</param>
		/// <returns>Elementos posteriores</returns>
		public static IEnumerable<T> PosterioresInclusivo<T>(this IEnumerable<T> lista, T atual) where T : class
		{
			if (lista == null)
			{
				throw new ArgumentNullException("lista");
			}

			if (!lista.Contains(atual))
			{
				throw new InvalidOperationException("O elemento atual não existe na lista");
			}

			return lista.SkipWhile(p => !p.Equals(atual));
		}

		/// <summary>
		/// Obtem o anterior item na lista
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="atual">Elemento atual</param>
		/// <returns>Próximo elemento caso o atual não seja o último</returns>
		public static T Anterior<T>(this IEnumerable<T> lista, T atual) where T : class
		{
			if (!lista.Contains(atual))
			{
				return null;
			}

			return lista.TakeWhile(p => !p.Equals(atual)).LastOrDefault();
		}

		/// <summary>
		/// Obtem o próximo item na lista
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="atual">Elemento atual</param>
		/// <returns>Elemento anterior caso o atual não seja o primeiro</returns>
		public static T Posterior<T>(this IEnumerable<T> lista, T atual) where T : class
		{
			if (!lista.Contains(atual))
			{
				return null;
			}

			return lista.SkipWhile(p => !p.Equals(atual)).ElementAtOrDefault(1);
		}

		/// <summary>
		/// Obtem uma lista de sequencias
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <typeparam name="R">Tipo do agrupamento</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <param name="groupCondition">Condição para agrupamento</param>
		/// <returns>Lista das sequencias</returns>
		public static IEnumerable<Tuple<R, List<T>>> GroupBySequence<T, R>(this IEnumerable<T> lista, Func<T, R> groupCondition) where T : class
		{
			var sequences = new List<Tuple<R, List<T>>>();

			foreach (T item in lista)
			{
				var last = sequences.LastOrDefault();

				R condition = groupCondition(item);

				if ((sequences.Count == 0 || last.Item2.Count > 0) && (!condition.Equals(groupCondition(last.Item2.Last()))))
				{
					sequences.Add(new Tuple<R, List<T>>(condition, new List<T>()));
				}

				sequences.Last().Item2.Add(item);
			}

			return sequences;
		}

		/// <summary>
		/// Converte uma lista para string, concatenando cada item da lista
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="items">Lista de items</param>
		/// <param name="acao">Ação que será aplicada para cada item</param>
		/// <param name="separador">Separador dos itens</param>
		/// <returns>Texto concatenado</returns>
		public static string ToString<T>(this IEnumerable<T> items, Func<T, string> acao, string separador)
		{
			return string.Join(separador, items.Select(acao));
		}

		/// <summary>
		/// Converte uma lista para string separados por vírgula
		/// </summary>
		/// <typeparam name="T">Tipo do item</typeparam>
		/// <param name="items">Lista de items</param>
		/// <param name="acao">Ação que será aplicada para cada item</param>
		/// <returns>Texto concatenado</returns>
		public static string ToString<T>(this IEnumerable<T> items, Func<T, string> acao)
		{
			return items.ToString(acao, ", ");
		}

		/// <summary>
		/// Retorna uma lista distinta
		/// </summary>
		/// <typeparam name="TSource">Tipo de origem</typeparam>
		/// <typeparam name="TKey">Tipo do distinct</typeparam>
		/// <param name="source">Origem da lista</param>
		/// <param name="keySelector">Condição do distinct</param>
		/// <returns>Lista distinta</returns>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> knownKeys = new HashSet<TKey>();

			foreach (TSource element in source)
			{
				if (knownKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		/// <summary>
		/// Obtem a lista randomizada
		/// </summary>
		/// <typeparam name="T">Tipo do parâmetro</typeparam>
		/// <param name="lista">Lista de itens</param>
		/// <returns>Lista randomizada</returns>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> lista) where T : class
		{
			return lista.OrderBy(i => Guid.NewGuid());
		}

		/// <summary>
		/// Verify if the number is in the range
		/// </summary>
		/// <param name="value">number value</param>
		/// <param name="minimum">range minimum</param>
		/// <param name="maximum">maximum number</param>
		/// <returns>true if is in the range</returns>
		public static bool IsWithin(this int value, int minimum, int maximum)
		{
			return value >= minimum && value <= maximum;
		}
	}
}

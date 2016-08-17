using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace VSTS
{
	/// <summary>
	/// https://code.msdn.microsoft.com/windowsdesktop/Samples-for-Parallel-b4b76364/sourcecode?fileId=44488&pathId=197768125
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public sealed class ObservableConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, INotifyCollectionChanged
	{
		public ObservableConcurrentDictionary()
		{
		}

		public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
			: base(collection)
		{
		}

		public ObservableConcurrentDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
			: base(concurrencyLevel, capacity)
		{
		}

		public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
			: base(collection, comparer)
		{
		}

		public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
			: base(concurrencyLevel, capacity, comparer)
		{
		}

		public ObservableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
			: base(concurrencyLevel, collection, comparer)
		{
		}

		public new TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue value;

			if (ContainsKey(key))
			{
				TValue oldValue;
				var success = TryGetValue(key, out oldValue);

				if (!success)
				{
					throw new InvalidOperationException(string.Format("Could not obtain old value for key '{0}'", key));
				}

				value = base.AddOrUpdate(key, addValue, updateValueFactory);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldValue)));
			}
			else
			{
				value = base.AddOrUpdate(key, addValue, updateValueFactory);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return value;
		}

		public new TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			TValue value;

			if (ContainsKey(key))
			{
				TValue oldValue;
				var success = TryGetValue(key, out oldValue);

				if (!success)
				{
					throw new InvalidOperationException(string.Format("Could not obtain old value for key '{0}'", key));
				}

				value = base.AddOrUpdate(key, addValueFactory, updateValueFactory);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldValue)));
			}
			else
			{
				value = base.AddOrUpdate(key, addValueFactory, updateValueFactory);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return value;
		}

		public new void Clear()
		{
			base.Clear();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public new TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			TValue value;

			if (ContainsKey(key))
			{
				value = base.GetOrAdd(key, valueFactory);
			}
			else
			{
				value = base.GetOrAdd(key, valueFactory);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return value;
		}

		public new TValue GetOrAdd(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				base.GetOrAdd(key, value);
			}
			else
			{
				base.GetOrAdd(key, value);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return value;
		}

		public new bool TryAdd(TKey key, TValue value)
		{
			var addSuccessful = base.TryAdd(key, value);

			if (addSuccessful)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return addSuccessful;
		}

		public new bool TryRemove(TKey key, out TValue value)
		{
			var removeSuccessful = base.TryRemove(key, out value);

			if (removeSuccessful)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
			}

			return removeSuccessful;
		}

		public new bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
		{
			var oldValue = default(TValue);

			if (ContainsKey(key))
			{
				var getSuccessful = TryGetValue(key, out oldValue);

				if (!getSuccessful)
				{
					throw new InvalidOperationException(string.Format("Could not obtain old value for key '{0}'", key));
				}
			}

			var updateSuccessful = base.TryUpdate(key, newValue, comparisonValue);

			if (updateSuccessful)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, newValue), new KeyValuePair<TKey, TValue>(key, oldValue)));
			}

			return updateSuccessful;
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			var handler = CollectionChanged;
			handler?.Invoke(this, e);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}

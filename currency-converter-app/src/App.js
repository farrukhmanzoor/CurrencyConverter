import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';
import LatestRates from './components/LatestRates';
import CurrencyConverter from './components/CurrencyConverter';
import HistoricalRates from './components/HistoricalRates';
function App() {
  const [currencies, setCurrencies] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCurrencies = async () => {
      try {
        const response = await axios.get('https://localhost:44311/api/exchangerate/currencies');
        let d =Object.entries(response.data)
        .map(([key, value]) => `${key}: ${value}`);
        setCurrencies(d);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching currencies', error);
        setLoading(false);
      }
    };

    fetchCurrencies();
  }, []);

  if (loading) {
    return <p>Loading currencies...</p>;
  }
  return (
    <div className="App">
      <header className="App-header">
        <h1>Currency Converter App</h1>
      </header>
      <main>
        <LatestRates currencies={currencies}/>
        <CurrencyConverter currencies={currencies} />
        <HistoricalRates currencies={currencies} />
      </main>
    </div>
  );
}

export default App;

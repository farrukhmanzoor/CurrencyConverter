import React, { useState } from 'react';
import axios from 'axios';

const LatestRates = () => {
  const [baseCurrency, setBaseCurrency] = useState('USD');
  const [rates, setRates] = useState(null);
  const [loading, setLoading] = useState(false);

  const fetchLatestRates = async () => {
    setLoading(true);
    try {
      const response = await axios.get(`https://localhost:44311/api/ExchangeRate/latest?baseCurrency=${baseCurrency}`);
      setRates(response.data);
    } catch (error) {
      console.error('Error fetching latest rates', error);
    }
    setLoading(false);
  };

  return (
    <div className="card">
      <h2>Get Latest Rates</h2>
      <input
        type="text"
        value={baseCurrency}
        onChange={(e) => setBaseCurrency(e.target.value)}
        placeholder="Base Currency (e.g. USD)"
      />
      <button onClick={fetchLatestRates}>Fetch Latest Rates</button>

      {loading ? (
        <p>Loading...</p>
      ) : (
        rates && (
          <div>
            <h3>Rates for {baseCurrency}:</h3>
            <ul>
              {Object.entries(rates.rates).map(([currency, rate]) => (
                <li key={currency}>
                  {currency}: {rate}
                </li>
              ))}
            </ul>
          </div>
        )
      )}
    </div>
  );
};

export default LatestRates;

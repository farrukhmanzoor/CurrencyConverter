import React, { useState } from 'react';
import axios from 'axios';

const CurrencyConverter = ({ currencies }) =>{
  const [fromCurrency, setFromCurrency] = useState('USD');
  const [toCurrency, setToCurrency] = useState('EUR');
  const [amount, setAmount] = useState(1);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const convertCurrency = async () => {
    setLoading(true);
    try {
        //
      const response = await axios.get(`https://localhost:44311/api/exchangerate/convert?fromCurrency=${fromCurrency}&toCurrency=${toCurrency}&amount=${amount}`);
      setResult(response.data);
    } catch (error) {
      console.error('Error converting currency', error);
    }
    setLoading(false);
  };

  return (
    <div className="card">
      <h2>Convert Currency</h2>
      <select value={fromCurrency} onChange={(e) => setFromCurrency(e.target.value)}
        placeholder="From Currency"
        >
        {currencies.map((currency) => (
          <option key={currency} value={currency}>
            {currency}
          </option>
        ))}
      </select>
      
      <select 
      value={toCurrency} 
      onChange={(e) => setToCurrency(e.target.value)}
      placeholder="To Currency"  
      >
        {currencies.map((currency) => (
          <option key={currency} value={currency}>
            {currency}
          </option>
        ))}
      </select>
      <input
        type="number"
        value={amount}
        onChange={(e) => setAmount(e.target.value)}
        placeholder="Amount"
      />
      <button onClick={convertCurrency}>Convert</button>

      {loading ? <p>Loading...</p> : result &&
      (
        <div>
          <h3>Rates for {fromCurrency}:</h3>
          <ul>
            {Object.entries(result.rates).map(([currency, rate]) => (
              <li key={currency}>
                {currency}: {rate}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default CurrencyConverter;

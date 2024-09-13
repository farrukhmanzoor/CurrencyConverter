import React from 'react';
import './App.css';
import LatestRates from './components/LatestRates';
import CurrencyConverter from './components/CurrencyConverter';
import HistoricalRates from './components/HistoricalRates';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Currency Converter App</h1>
      </header>
      <main>
        <LatestRates />
        <CurrencyConverter />
        <HistoricalRates />
      </main>
    </div>
  );
}

export default App;

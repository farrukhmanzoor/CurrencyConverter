import React, { useState } from 'react';
import axios from 'axios';

const HistoricalRates = () => {
  const [baseCurrency, setBaseCurrency] = useState('USD');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [rates, setRates] = useState(null);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5); // Fixed page size

  const fetchHistoricalRates = async (pageNum = 1) => {
    setLoading(true);
    try {
      const response = await axios.get(
        `https://localhost:44311/api/exchangerate/history?baseCurrency=${baseCurrency}&startDate=${startDate}&endDate=${endDate}&page=${pageNum}&pageSize=${pageSize}`
      );
      setRates(response.data);
      setPage(pageNum); // Update the current page
    } catch (error) {
      console.error('Error fetching historical rates', error);
    }
    setLoading(false);
  };

// Function to format the date
const formatDateTime = (dateTime) => {
console.log(dateTime);
    const date = new Date(dateTime);
    console.log(date);
    return date.toDateString(); // Returns 'YYYY-MM-DD HH:MM:SS'
  };


  // Function to go to the next page
  const handleNextPage = () => {
    fetchHistoricalRates(page + 1);
  };

  // Function to go to the previous page
  const handlePrevPage = () => {
    if (page > 1) {
      fetchHistoricalRates(page - 1);
    }
  };

  return (
    <div className="card">
      <h2>Get Historical Rates</h2>
      <input
        type="text"
        value={baseCurrency}
        onChange={(e) => setBaseCurrency(e.target.value)}
        placeholder="Base Currency (e.g. USD)"
      />
      <input
        type="date"
        value={startDate}
        onChange={(e) => setStartDate(e.target.value)}
        placeholder="Start Date"
      />
      <input
        type="date"
        value={endDate}
        onChange={(e) => setEndDate(e.target.value)}
        placeholder="End Date"
      />
      <button onClick={() => fetchHistoricalRates(1)}>Fetch Historical Rates</button>

      {loading ? (
        <p>Loading...</p>
      ) : (
        rates && (
          <div>
            <h3>Historical Rates for {baseCurrency} from </h3>
            <h4>{formatDateTime( startDate)} to {formatDateTime(endDate)}</h4>
            <div className="table-container">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Currency</th>
                    <th>Rate</th>
                  </tr>
                </thead>
                <tbody>
                  {Object.entries(rates.rates).map(([date, rateData]) => (
                    <React.Fragment key={date}>
                      {Object.entries(rateData).map(([currency, rate], index) => (
                        <tr key={index}>
                          <td>{index === 0 ? date : ''}</td> {/* Group by date */}
                          <td>{currency}</td>
                          <td>{rate}</td>
                        </tr>
                      ))}
                    </React.Fragment>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="pagination">
              <button onClick={handlePrevPage} disabled={page === 1}>
                Previous
              </button>
              <span> Page {page} </span>
              <button onClick={handleNextPage}>
                Next
              </button>
            </div>
          </div>
        )
      )}
    </div>
  );
};

export default HistoricalRates;

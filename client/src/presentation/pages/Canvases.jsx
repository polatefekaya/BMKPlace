import 'bootstrap/dist/css/bootstrap.min.css';
import {Col, Row } from 'react-bootstrap';
import CardItem from '../components/canvasListItem';
import "../styles/texts.css";
import SearchBar from '../components/searchBar';
import { useState } from 'react';

export default function Canvases () {
    const initialData = [
        { name: 'Project A', creator: 'Alice', date: '2024-10-01', icon: 'https://via.placeholder.com/50' },
        { name: 'Project B', creator: 'Bob', date: '2024-10-02', icon: 'https://via.placeholder.com/50' },
        { name: 'Project C', creator: 'Charlie', date: '2024-10-03', icon: 'https://via.placeholder.com/50' },
        { name: 'Project D', creator: 'Diana', date: '2024-10-04', icon: 'https://via.placeholder.com/50' },
        { name: 'Project E', creator: 'Edward', date: '2024-10-05', icon: 'https://via.placeholder.com/50' },
        { name: 'Project F', creator: 'Fiona', date: '2024-10-06', icon: 'https://via.placeholder.com/50' },
        { name: 'Project G', creator: 'George', date: '2024-10-07', icon: 'https://via.placeholder.com/50' },
        { name: 'Project H', creator: 'Hannah', date: '2024-10-08', icon: 'https://via.placeholder.com/50' },
        { name: 'Project I', creator: 'Ian', date: '2024-10-09', icon: 'https://via.placeholder.com/50' },
        { name: 'Project J', creator: 'Jane', date: '2024-10-10', icon: 'https://via.placeholder.com/50' },
      ];

      const [searchTerm, setSearchTerm] = useState('');
      const [filterOption, setFilterOption] = useState('All');
      const [sortOption, setSortOption] = useState('Date');
    
      const handleSearchChange = (term) => setSearchTerm(term);
      const handleFilterChange = (filter) => setFilterOption(filter);
      const handleSortChange = (sort) => setSortOption(sort);
    
      const filteredAndSortedCards = initialData
        .filter((card) =>
          filterOption === 'All' || card.creator.toLowerCase().includes(filterOption.toLowerCase())
        )
        .filter((card) =>
          card.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          card.creator.toLowerCase().includes(searchTerm.toLowerCase())
        )
        .sort((a, b) => (sortOption === 'Name' ? a.name.localeCompare(b.name) : new Date(a.date) - new Date(b.date)));
    

    return (
    <>
    <Col style={{overflow:"hidden"}}>
    <div className="container mt-4 mb-3">
      <h1 className="mb-4 text-center">Canvas List</h1>

      {/* Render Search Bar */}
      <SearchBar
        searchTerm={searchTerm}
        onSearchChange={handleSearchChange}
        filterOption={filterOption}
        onFilterChange={handleFilterChange}
        sortOption={sortOption}
        onSortChange={handleSortChange}
      />

      {/* Render Filtered and Sorted Cards */}
      <Row className="g-3">
        {initialData.map((card, index) => (
          <Col xs={12} lg={6}  xxl={4} key={index}>
            <CardItem
              name={card.name}
              creator={card.creator}
              date={card.date}
              icon={card.icon}
              id={index}
            />
          </Col>
        ))}
      </Row>
    </div>
    </Col>
    </>);
}
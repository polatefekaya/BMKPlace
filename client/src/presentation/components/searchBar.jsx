import React from 'react';
import { Form, Dropdown, ButtonGroup, DropdownButton } from 'react-bootstrap';
import { FaSearch, FaFilter, FaSort } from 'react-icons/fa';
import '../styles/SearchBar.css'; // Import the CSS

function SearchBar({
  searchTerm,
  onSearchChange,
  filterOption,
  onFilterChange,
  sortOption,
  onSortChange,
}) {
  return (
    <div className="search-bar-container mb-3">
      <div className="search-input-wrapper">
        {/* Search Icon and Input */}
        <FaSearch className="search-icon" />
        <Form.Control
          type="text"
          placeholder="Search"
          value={searchTerm}
          onChange={(e) => onSearchChange(e.target.value)}
          className="search-input"
        />
      </div>

      {/* Filter and Sort Dropdowns */}
      <div className="dropdown-wrapper">


        <Dropdown as={ButtonGroup}>
          <DropdownButton
            title={`Sort: ${sortOption}`}
            variant="outline-secondary"
            className="dropdown-btn"
          >
            <Dropdown.Item onClick={() => onSortChange('Name')}>Name</Dropdown.Item>
            <Dropdown.Item onClick={() => onSortChange('Date')}>Date</Dropdown.Item>
          </DropdownButton>
        </Dropdown>
      </div>
    </div>
  );
}

export default SearchBar;

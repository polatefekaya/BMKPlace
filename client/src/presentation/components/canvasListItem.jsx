import React from 'react';
import { Card, Row, Col, Image } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

function CardItem({ name, creator, date, icon, id }) {
    const navigate = useNavigate(); // Initialize the navigation hook

    const handleCardClick = () => {
      navigate(`/canvas/${id}`); // Navigate to the detail page
    };
  return (
    <Card className="mb-0 shadow-sm rounded" onClick={handleCardClick}>
      <Card.Body>
        <Row className="align-items-center">
          <Col xs={3} md={2} className="text-center">
            <Image src={icon} roundedCircle width={50} height={50} />
          </Col>
          <Col xs={6} md={7}>
            <Card.Title className="mb-1">{name}</Card.Title>
            <Card.Subtitle className="text-muted small">
              Created by {creator}
            </Card.Subtitle>
          </Col>
          <Col xs={3} md={3} className="text-end">
            <span className="text-muted small">{date}</span>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
}

export default CardItem;

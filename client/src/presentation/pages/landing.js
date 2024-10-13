import 'bootstrap/dist/css/bootstrap.min.css';
import { Button, Col, Container, Nav, Row, Stack } from 'react-bootstrap';
import "../styles/texts.css";
import { useNavigate } from 'react-router-dom';

export default function Landing () {
    const navigate = useNavigate();

    const goToCanvases = () => {
        navigate("/canvases");
    }

    return (
    <>
    <Col style={{overflow:"hidden"}}>
        <Row>
            <div style={{height: 60}}></div>
        </Row>
        <Row>
            <Container className='pt-5'>
            <Container className='mt-5'>
                <h1 className='text-center'>Your Online</h1>
                <h1 className='text-center display-1' style={{fontWeight: "600"}}>Canvas Platform</h1>
            </Container>
            </Container>
            <Row style={{paddingRight: 0}}>
                <Col></Col>
                <Col xs={6} lg={4} xxl={2} style={{paddingRight: 0}}>
                    <Stack gap={1} className='mt-5'>
                        <Button onClick={goToCanvases}>Go to Canvases</Button>
                        <div className='text-center mt-1'>or</div>
                        <Button variant='link'>Create Yours</Button>
                    </Stack>
                </Col>
                <Col></Col>
            </Row>
            <Nav variant="body" className="bg-body-tertiary fixed-bottom">
            <Container>
                <h6 className='text-body-secondary mt-2'>Beykent University</h6>
                <h5 className=''>Bilgisayar Mühendisliği Kulübü</h5>
            </Container>
            </Nav>
            
        </Row>
        <Row></Row>
    </Col>
    </>);
}
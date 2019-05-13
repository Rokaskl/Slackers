import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';
import Chart from 'react-apexcharts'

export class AdminChart extends React.Component {
  constructor(props) {
    super(props);
 
    this.state = {
      options: {
        chart: {
            width: "100%",
            height: 50,
            type: "bar"
            
        },
        xaxis: {
          categories: ["Mon", "Tue", "Wed", "Thu" , "Fri", "Sat", "Sun"]
        }
      },
      series: [{
        type : 'bar',
        name: 'Work Hours',
        data: [8, 4, 0, 12, 8, 6, 7]
      },
      {
        type : 'bar',
        name: 'Work Hours',
        data: [6, 7, 4, 8, 12, 7, 12]
      }
      ,
      {
        type : 'bar',
        name: 'Work Hours',
        data: [0, 6, 12, 3, 5, 6, 10]
      }
      ,
      {
        type : 'bar',
        name: 'Work Hours',
        data: [7, 3, 10, 4, 1, 15, 12]
      },
      {
        type : 'line',
        name: 'Avarage',
        data: [5, 5, 8, 6, 4, 9, 10]
      }]
    }
  }
  render() {
    return (
      <div>
      <Chart options={this.state.options} series={this.state.series}   />
      </div>
    )
  }
}
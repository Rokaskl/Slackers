import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';
import Chart from 'react-apexcharts'

export class AdminChart extends React.Component {
  constructor(props) {
    super(props);
      }
    render() {
      var times = this.props.times
      var options ={
        chart: {
//height: 380,
          width: "100%",
          type: "area",
          animations: {
            initialAnimation: {
              enabled: true
            }
          }
        },
        xaxis: {
          type: "datetime"
        },
        dataLabels: {
          enabled: true,
          formatter: function (val, opt) {
            return   ~~(val/60)+":"+(val%60)
          }
        }
      };
      var series = [ ];
      var options2 = {
        labels: [],

        chart: {
            type: 'donut',
        },
        responsive: [{
            breakpoint: 480,
            options: {
                chart: {
                    width: 200
                },
                legend: {
                    position: 'bottom'
                }
            }
        }]
    }
    var series2= [];
    var serie;
    var names = [];


    {times.loading && <em>Loading rooms...</em>}
    {times.error && <span className="text-danger">ERROR: {times.error}</span>}
    {times.items &&
       <div>
          {times.items.map((time, index) =>
            <div key={time.name}>  
              {serie = 0}
                {time.data.map((subtime =>
                  <div key={subtime.x}>
                    ({serie+=subtime.y}
                  </div>  
               ))}
               {console.log("Donut chart serie number : " + serie + "  Donut chart serie name : " + time.name)}
                {series.push({name:time.name, data: time.data})}
               {names.push(time.name.toString())}
               {series2.push(serie)}
              </div>      
           )}
                                  
       </div>
     }
     {Object.assign(options2.labels,names)}
     {console.log("DONUT Labels : " + options2.labels + " Series : " + series2 )}

      return (
         <div> 
        <Chart options={options} series={series} type="bar" height="550" />
         <h2>In Total</h2>
        <Chart options={options2} series={series2} type="donut" height="450"  />
         </div> 
      );
  }
}

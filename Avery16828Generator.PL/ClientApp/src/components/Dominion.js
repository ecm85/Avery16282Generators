import React, { Component } from 'react';
//import Checkbox from "./Checkbox";

export class Dominion extends Component {
    displayName = Dominion.name

  constructor(props) {
    super(props);
      this.state = { initializing: true, loading: false};
      fetch('api/PdfGenerator/GetDominionExpansions')
          .then(response =>
              response.json())
          .then(data => {
              this.setState({ expansions: data, initializing: false });
          });
     }

    render() {
        let contents = null;
        if (this.state.initializing) {
            contents = <p><em>Initializing...</em></p>
        }
        else {
            contents =
                <form  method='post' action='api/PdfGenerator/GenerateDominion'>
                <select name='expansionNames' className='selectmultiple form-control' multiple='multiple' size={this.state.expansions.length}>
                    {this.state.expansions.map((expansion => <option key={expansion.text}>{expansion.text}</option>))}
                </select>
                <input type='submit' className='btn btn-primary' value='Generate Labels'></input>
                </form>
            }
        
        return (
          <div>
            <h1>Dominion</h1>
            {contents}
          </div>
        );
  }
}

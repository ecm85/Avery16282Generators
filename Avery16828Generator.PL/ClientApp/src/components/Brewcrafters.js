import React, { Component } from 'react';

export class Brewcrafters extends Component {
    displayName = Brewcrafters.name

  constructor(props) {
    super(props);
      this.state = { loading: false};
  }
    
    render() {
    return (
      <div>
        <h1>Brewcrafters</h1>
        <form method='post' action='api/PdfGenerator/GenerateBrewcrafters'>
                <input type='submit' className='btn btn-primary' value='Generate Labels'></input>
        </form>
      </div>
    );
  }
}

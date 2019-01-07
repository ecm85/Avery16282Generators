import React, { Component } from 'react';

export class Dominion extends Component {
    displayName = Dominion.name

  constructor(props) {
    super(props);
      this.state = { initializing: true, filename: null, loading: false};
      this.generateDominion = this.generateDominion.bind(this);
      fetch('api/PdfGenerator/GetDominionExpansions')
          .then(response =>
              response.json())
          .then(data => {
              this.setState({ dominionExpansions: data, initializing: false });
          });
     }

    generateDominion() {
        this.setState({ filename: null, loading: true });
        fetch('api/PdfGenerator/GenerateDominion',
                {
                    method: 'post',
                    headers: {
                        'Accept': 'application/json, text/plain, */*',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.state.dominionExpansions)
                })
            .then(response =>
                response.text())
            .then(data => {
                this.setState({ filename: data, loading: false });
            });
    }
 
    render() {
        let contents = null;
        if (this.state.initializing) {
            contents = <p><em>Initializing...</em></p>
        }
        else if (this.state.loading) {
            contents = <p><em>Creating...</em></p>
        }
        else if (this.state.filename === null) {
                contents = <button onClick={this.generateDominion}>Generate Labels</button>
                }
        else {
            contents =
                <div>
                    <button onClick={this.generateDominion}>Generate Labels</button>
                    <div><a download href={"api/PdfGenerator/GetFile?fileName=" + this.state.filename}>Download PDF</a></div>
                </div>
            }
        
        return (
          <div>
            <h1>Dominion</h1>
            {contents}
          </div>
        );
  }
}

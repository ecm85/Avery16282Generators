import React, { Component } from 'react';
import Checkbox from "./Checkbox";

export class Dominion extends Component {
    displayName = Dominion.name

  constructor(props) {
    super(props);
      this.state = { initializing: true, filename: null, loading: false};
      this.generateDominion = this.generateDominion.bind(this);
      this.createCheckbox = this.createCheckbox.bind(this);
      this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
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

    createCheckbox = function(option) {
        return <Checkbox
        label = { option.text }
        isSelected = { option.includeExpansion }
        onCheckboxChange = { () => this.handleCheckboxChange(option) }
        key = { option.expansion } /> 
    }

    handleCheckboxChange = function (option) {
        this.setState({
            dominionExpansions: this.state.dominionExpansions.map(expansion =>
                (expansion === option ?
                    Object.assign({}, expansion, { includeExpansion: !option.includeExpansion }) :
                    expansion))
        });
    };
 
    render() {
        let contents = null;
        if (this.state.initializing) {
            contents = <p><em>Initializing...</em></p>
        }
        else if (this.state.loading) {
            contents = <p><em>Creating...</em></p>
        }
        else if (this.state.filename === null) {
            contents =
                <div>
                    <button onClick={this.generateDominion}>Generate Labels</button>
                    {this.state.dominionExpansions.map(this.createCheckbox)}
                </div>
        }
        else {
            contents =
                <div>
                    <button onClick={this.generateDominion}>Generate Labels</button>
                    {this.state.dominionExpansions.map(this.createCheckbox)}
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

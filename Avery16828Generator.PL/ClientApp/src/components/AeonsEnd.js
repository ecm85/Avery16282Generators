import React, { Component } from 'react';

export class AeonsEnd extends Component {
    displayName = AeonsEnd.name

    constructor(props) {
        super(props);
        this.state = { initializing: true, loading: false };
        fetch('api/PdfGenerator/GetAeonsEndExpansions')
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
            <form method='post' action='api/PdfGenerator/GenerateAeonsEnd'>
                <select name='expansionNames' className='selectmultiple form-control' defaultValue={['Aeon\'s End']} multiple='multiple' size={this.state.expansions.length}>
                {this.state.expansions.map((expansion => <option key={expansion}>{expansion}</option>))}
                </select>
                <input type='submit' className='btn btn-primary' value='Generate Labels'></input>
            </form>
        }

        return (
            <div>
            <h1>Aeons End</h1>
            {contents}
            </div>
        );
    }
}

import React, { Component } from 'react';

export class Home extends Component {
  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Avery 16282 Labels</h1>
            <p>This site contains generators for PDF files to print onto <a href="https://www.avery.com/products/tabs/16282">https://www.avery.com/products/tabs/16282</a>.</p>
            <p>They can be attached to plastic dividers or envelopes for various games to provide organization.</p>
            <p>This site can generate labels for:</p>
       <ul>
          <li><strong>Brewcrafters</strong>. To organize the beer tokens along with the gold label and card.</li>
          <li><strong>Dominion</strong>. Supports choosing which expansions to print labels for.</li>
          <li><strong>Aeon's End</strong>. Supports choosing which expansions to print labels for. Generates labels for each gem, relic, spell and boss.</li>
          <li><strong>Legendary</strong>.Supports choosing which expansions to print labels for. Also supports optionally creating labels for special cards (i.e. special wounds, special bystanders, etc).</li>
        </ul>
      </div>
    );
  }
}

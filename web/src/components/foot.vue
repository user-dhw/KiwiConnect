<template>
	<div class="foot">
		<!-- Start of Footer -->
		<footer id="footer-wrapper">
			<div id="footer" class="container">
				<div class="row">
					<div class="span3">
						<section class="widget">
							<h3 class="title">Main Features</h3>
							<div class="textwidget">
								<p>
									KiwiConnect is the information sharing and
									communication platform. After registering an
									account, users can post information and
									comments, seek help, and assist others.
								</p>
							</div>
							<h3 class="title">Announcements</h3>
							<div class="textwidget">
								<p>
									Inappropriate content is prohibited on this
									platform.<br />
									Abusive language is not allowed and will
									result in account suspension.
								</p>
							</div>
						</section>
					</div>
					<div class="span3">
						<section class="widget">
							<h3 class="title">Advertisements</h3>
							<div id="twitter_update_list">
								<ul>
									<li>No ads currently</li>
								</ul>
							</div>
						</section>
					</div>
					<div class="span3">
						<section class="widget">
							<h3 class="title">Customer Service</h3>
							<ul>
								<li>
									<router-link to="/feedback" title="Feedback"
										>Feedback</router-link
									>
								</li>
								<li>
									<router-link
										to="/appeal"
										title="Account Appeal"
										>Account Appeal</router-link
									>
								</li>
								<li>
									<router-link
										to="/report"
										title="Report Misconduct"
										>Report Misconduct</router-link
									>
								</li>
							</ul>
						</section>
					</div>
				</div>
			</div>
			<!-- end of #footer -->
		</footer>
		<!-- End of Footer -->
	</div>
</template>

<script setup>
// Footer component - Vue 3
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.foot {
	width: 100%;
	bottom: 0;
}

#footer-wrapper {
	background-color: #2c3e50;
	color: #c1cad1;
	padding: 40px 0;
	margin-top: 50px;
}

#footer {
	padding: 20px 0;
}

.row {
	display: flex;
	flex-wrap: wrap;
	gap: 20px;
}

.span3 {
	flex: 1;
	min-width: 200px;
}

.widget {
	margin-bottom: 20px;
}

.widget .title {
	font-size: 16px;
	font-weight: 600;
	color: #fff;
	margin-bottom: 15px;
	padding-bottom: 10px;
	border-bottom: 2px solid #545c64;
}

.widget .textwidget {
	font-size: 14px;
	line-height: 1.6;
	color: #c1cad1;
}

.widget ul {
	list-style: none;
	padding: 0;
	margin: 0;
}

.widget ul li {
	margin-bottom: 10px;
}

.widget ul li a {
	color: #c1cad1;
	text-decoration: none;
	font-size: 14px;
	transition: color 0.3s;
}

.widget ul li a:hover {
	color: #fff;
	text-decoration: underline;
}

#twitter_update_list ul li {
	color: #c1cad1;
	font-size: 14px;
	margin-bottom: 8px;
}

@media (max-width: 768px) {
	.span3 {
		min-width: 100%;
	}

	.row {
		flex-direction: column;
	}
}
</style>

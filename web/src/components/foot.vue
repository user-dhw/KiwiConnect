<template>
	<div class="foot">
		<!-- Start of Footer -->
		<footer id="footer-wrapper">
			<div id="footer" class="container">
				<div class="footer-brand">
					<div class="footer-brand-mark">KC</div>
					<div class="footer-brand-copy">
						<div class="footer-brand-title">KiwiConnect</div>
						<div class="footer-brand-subtitle">
							University of Waikato community hub
						</div>
					</div>
				</div>
				<div class="footer-grid">
					<div class="footer-column">
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
					<div class="footer-column">
						<section class="widget">
							<h3 class="title">Advertisements</h3>
							<div id="twitter_update_list">
								<ul>
									<li>No ads currently</li>
								</ul>
							</div>
						</section>
					</div>
					<div class="footer-column">
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
	background:
		radial-gradient(circle at top left, rgba(38, 99, 235, 0.2), transparent 22%),
		linear-gradient(135deg, #1c49b7 0%, #2663eb 58%, #1749c6 100%);
	color: #dfe8ff;
	padding: 56px 0 48px;
	margin-top: 56px;
}

#footer {
	padding: 20px 0;
}

.footer-brand {
	display: flex;
	align-items: center;
	gap: 16px;
	margin-bottom: 28px;
	padding: 0 0 6px;
}

.footer-brand-mark {
	width: 50px;
	height: 50px;
	border-radius: 16px;
	display: grid;
	place-items: center;
	background: linear-gradient(135deg, #ffffff, #dbe7ff);
	color: #2663eb;
	font-size: 20px;
	font-weight: 800;
	letter-spacing: 0.08em;
	box-shadow: 0 14px 30px rgba(19, 53, 137, 0.24);
}

.footer-brand-title {
	font-size: 24px;
	font-weight: 800;
	line-height: 1;
	color: #ffffff;
	letter-spacing: -0.03em;
}

.footer-brand-subtitle {
	margin-top: 6px;
	font-size: 13px;
	color: rgba(235, 242, 255, 0.82);
}

.row {
	display: flex;
	flex-wrap: wrap;
	gap: 24px;
}

.footer-grid {
	display: grid;
	grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
	gap: 24px;
}

.widget {
	margin-bottom: 20px;
	padding: 24px;
	border-radius: 22px;
	background: rgba(255, 255, 255, 0.08);
	border: 1px solid rgba(255, 255, 255, 0.12);
	box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.03);
}

.widget .title {
	font-size: 16px;
	font-weight: 600;
	color: #fff;
	margin-bottom: 15px;
	padding-bottom: 10px;
	border-bottom: 1px solid rgba(255, 255, 255, 0.12);
}

.widget .textwidget {
	font-size: 14px;
	line-height: 1.6;
	color: #dfe8ff;
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
	color: #e7efff;
	text-decoration: none;
	font-size: 14px;
	transition: color 0.3s;
}

.widget ul li a:hover {
	color: #fff;
	text-decoration: none;
}

#twitter_update_list ul li {
	color: #e7efff;
	font-size: 14px;
	margin-bottom: 8px;
}

@media (max-width: 768px) {
	.footer-brand {
		align-items: flex-start;
	}

	.widget {
		padding: 20px;
	}
}
</style>
